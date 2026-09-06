using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingLifecycleCompleteServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IBookingProcedureRepository> _bookingProcedureRepoMock;

        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IWalkInQueueService> _queueServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<ILoyaltyTierService> _loyaltyTierServiceMock;
        private readonly Mock<ILogger<BookingService>> _loggerMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IPromotionService> _promotionServiceMock;

        private readonly BookingLifecycleService _service;

        private readonly Guid _bookingId = Guid.NewGuid();
        private readonly Guid _actorId = Guid.NewGuid();

        public BookingLifecycleCompleteServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _bookingProcedureRepoMock = new Mock<IBookingProcedureRepository>();

            _mapperMock = new Mock<IMapper>();
            _queueServiceMock = new Mock<IWalkInQueueService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _loyaltyTierServiceMock = new Mock<ILoyaltyTierService>();
            _loggerMock = new Mock<ILogger<BookingService>>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _promotionServiceMock = new Mock<IPromotionService>();

            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingProcedureRepository).Returns(_bookingProcedureRepoMock.Object);

            _service = new BookingLifecycleService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _queueServiceMock.Object,
                _bookingSchedulingServiceMock.Object,
                _loyaltyTierServiceMock.Object,
                _loggerMock.Object,
                _bookingProcedureServiceMock.Object,
                _notificationServiceMock.Object,
                _promotionServiceMock.Object
            );
        }

        private CompleteServiceRequestDTO CreateRequest(List<string>? images = null)
        {
            return new CompleteServiceRequestDTO
            {
                BookingId = _bookingId,
                CompleteImagesUrl = images ?? new List<string> { "https://cdn.example.com/img1.jpg", "https://cdn.example.com/img2.jpg" }
            };
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID01_AllRequiredProceduresCompleted_ReturnsSuccess()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            var procedures = new List<BookingProcedure>
            {
                new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    ProcedureName = "Làm sạch móng",
                    IsRequired = true,
                    Status = BookingProcedureStatus.Completed
                }
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(procedures);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ServiceCompleted.ToString() });

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hoàn thành dịch vụ làm móng thành công");
            booking.Status.Should().Be(BookingStatus.ServiceCompleted);
            booking.CheckOutImagesUrl.Should().Be("https://cdn.example.com/img1.jpg,https://cdn.example.com/img2.jpg");
            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID02_AllRequiredProceduresSkipped_ReturnsSuccess()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            var procedures = new List<BookingProcedure>
            {
                new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    ProcedureName = "Dưỡng móng",
                    IsRequired = true,
                    Status = BookingProcedureStatus.Skipped
                }
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(procedures);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ServiceCompleted.ToString() });

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hoàn thành dịch vụ làm móng thành công");
            booking.Status.Should().Be(BookingStatus.ServiceCompleted);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID03_NoProceduresInBooking_ReturnsSuccess()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(new List<BookingProcedure>());

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ServiceCompleted.ToString() });

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hoàn thành dịch vụ làm móng thành công");
            booking.Status.Should().Be(BookingStatus.ServiceCompleted);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID04_BookingNotFound_ReturnsError()
        {
            // Arrange
            var request = CreateRequest();

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy thông tin đặt lịch");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID05_StatusCheckedIn_ReturnsError()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.CheckedIn
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể hoàn thành dịch vụ khi đơn đang ở trạng thái 'InProgress'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID06_StatusApprovedOrCompleted_ReturnsError()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.Completed
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể hoàn thành dịch vụ khi đơn đang ở trạng thái 'InProgress'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID07_IncompleteRequiredProcedure_ReturnsError()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            var procedures = new List<BookingProcedure>
            {
                new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    ProcedureName = "Sơn móng gel",
                    IsRequired = true,
                    Status = BookingProcedureStatus.Pending
                }
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(procedures);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Không thể hoàn thành dịch vụ. Các bước bắt buộc sau chưa hoàn thành: Sơn móng gel.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID08_DbException_ThrowsException()
        {
            // Arrange
            var request = CreateRequest();
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(new List<BookingProcedure>());

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("Database connection failed");
        }

        [Fact]
        public async Task CompleteServiceAsync_UTCID09_EmptyImagesList_ReturnsSuccess()
        {
            // Arrange
            var request = CreateRequest(images: new List<string>());
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(new List<BookingProcedure>());

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ServiceCompleted.ToString() });

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CompleteServiceAsync(request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hoàn thành dịch vụ làm móng thành công");
            booking.Status.Should().Be(BookingStatus.ServiceCompleted);
            booking.CheckOutImagesUrl.Should().Be(string.Empty);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
