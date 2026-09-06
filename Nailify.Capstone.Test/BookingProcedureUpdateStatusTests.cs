using AutoMapper;
using FluentAssertions;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Exceptions;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingProcedureUpdateStatusTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingProcedureRepository> _bookingProcedureRepoMock;
        private readonly Mock<INailArtistRepository> _nailArtistRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;

        private readonly BookingProcedureService _service;

        private readonly Guid _bookingProcedureId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();
        private readonly Guid _bookingItemId = Guid.NewGuid();
        private readonly Guid _bookingId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _customerUserId = Guid.NewGuid();

        public BookingProcedureUpdateStatusTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingProcedureRepoMock = new Mock<IBookingProcedureRepository>();
            _nailArtistRepoMock = new Mock<INailArtistRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _mapperMock = new Mock<IMapper>();
            _notificationServiceMock = new Mock<INotificationService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();

            _unitOfWorkMock.Setup(u => u.BookingProcedureRepository).Returns(_bookingProcedureRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_nailArtistRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);

            _service = new BookingProcedureService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _notificationServiceMock.Object,
                _bookingSchedulingServiceMock.Object
            );
        }

        private BookingProcedure CreateProcedure(int stepOrder = 1, BookingProcedureStatus status = BookingProcedureStatus.InProgress)
        {
            return new BookingProcedure
            {
                BookingProcedureId = _bookingProcedureId,
                BookingItemId = _bookingItemId,
                ProcedureName = "Làm sạch móng",
                StepOrder = stepOrder,
                Status = status,
                AssignedArtistId = _artistId,
                IsRequired = true,
                CanOverlap = false,
                ActiveDuration = 30,
                BookingItem = new BookingItem
                {
                    BookingItemId = _bookingItemId,
                    BookingId = _bookingId
                }
            };
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID01_StatusCompleted_NoNextArtist_ReturnsSuccess()
        {
            // Arrange
            var procedure = CreateProcedure(stepOrder: 1);

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _nailArtistRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<NailArtist, bool>>>()))
                .ReturnsAsync(true);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.Completed.ToString()
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.Completed);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Cập nhật trạng thái bước quy trình thành công.");
            procedure.Status.Should().Be(BookingProcedureStatus.Completed);
            procedure.CompletedAt.Should().NotBeNull();
            procedure.CompletedById.Should().Be(_artistId);
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID02_StatusCompleted_HasNextArtist_NotificationSuccess_ReturnsSuccess()
        {
            // Arrange
            var step1 = CreateProcedure(stepOrder: 1);

            var nextArtistAccountId = Guid.NewGuid();
            var nextArtistId = Guid.NewGuid();
            var step2 = new BookingProcedure
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingItemId = _bookingItemId,
                ProcedureName = "Sơn móng",
                StepOrder = 2,
                Status = BookingProcedureStatus.Pending,
                AssignedArtistId = nextArtistId,
                AssignedArtist = new NailArtist
                {
                    NailArtistId = nextArtistId,
                    AccountId = nextArtistAccountId
                }
            };

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(step1);

            _nailArtistRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<NailArtist, bool>>>()))
                .ReturnsAsync(true);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            var currentArtist = new NailArtist
            {
                NailArtistId = _artistId,
                Account = new User { FirstName = "Thanh", LastName = "DT" }
            };
            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(currentArtist);

            var booking = new Booking
            {
                BookingId = _bookingId,
                Customer = new Customer
                {
                    User = new User { FirstName = "Lan", LastName = "Nguyen" }
                }
            };
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, false))
                .ReturnsAsync(booking);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(nextArtistAccountId.ToString(), "NextStepReady", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.Completed.ToString()
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.Completed);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Cập nhật trạng thái bước quy trình thành công.");
            _notificationServiceMock.Verify(n => n.SendNotificationToUserAsync(nextArtistAccountId.ToString(), "NextStepReady", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID03_StatusInProgress_ReturnsSuccess()
        {
            // Arrange
            var procedure = CreateProcedure(stepOrder: 1, status: BookingProcedureStatus.Pending);

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _nailArtistRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<NailArtist, bool>>>()))
                .ReturnsAsync(true);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.InProgress.ToString()
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.InProgress);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Cập nhật trạng thái bước quy trình thành công.");
            procedure.Status.Should().Be(BookingProcedureStatus.InProgress);
            procedure.AssignedArtistId.Should().Be(_artistId);
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID04_ProcedureNotFound_ReturnsError()
        {
            // Arrange
            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync((BookingProcedure?)null);

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.Completed);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy bước quy trình yêu cầu.");
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID05_StatusCompleted_NotificationThrowsException_ReturnsSuccess()
        {
            // Arrange
            var step1 = CreateProcedure(stepOrder: 1);

            var nextArtistAccountId = Guid.NewGuid();
            var nextArtistId = Guid.NewGuid();
            var step2 = new BookingProcedure
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingItemId = _bookingItemId,
                ProcedureName = "Sơn móng",
                StepOrder = 2,
                Status = BookingProcedureStatus.Pending,
                AssignedArtistId = nextArtistId,
                AssignedArtist = new NailArtist
                {
                    NailArtistId = nextArtistId,
                    AccountId = nextArtistAccountId
                }
            };

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(step1);

            _nailArtistRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<NailArtist, bool>>>()))
                .ReturnsAsync(true);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(new NailArtist { Account = new User { FirstName = "Thanh", LastName = "DT" } });

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, false))
                .ReturnsAsync(new Booking { Customer = new Customer { User = new User { FirstName = "Lan", LastName = "Nguyen" } } });

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR connection dropped"));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.Completed.ToString()
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.Completed);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Cập nhật trạng thái bước quy trình thành công.");
            step1.Status.Should().Be(BookingProcedureStatus.Completed);
        }

        [Fact]
        public async Task UpdateProcedureStatusAsync_UTCID06_StatusCompleted_ConcurrencyException_ReturnsError()
        {
            // Arrange
            var procedure = CreateProcedure(stepOrder: 1);

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _nailArtistRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<NailArtist, bool>>>()))
                .ReturnsAsync(true);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new ConcurrencyException());

            // Act
            var result = await _service.UpdateProcedureStatusAsync(_bookingProcedureId, _artistId, BookingProcedureStatus.Completed);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Dữ liệu đã bị thay đổi bởi một tác vụ khác. Vui lòng tải lại trang.");
        }
    }
}
