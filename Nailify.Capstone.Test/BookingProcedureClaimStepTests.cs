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
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingProcedureClaimStepTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingProcedureRepository> _bookingProcedureRepoMock;
        private readonly Mock<INailArtistRepository> _nailArtistRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;

        private readonly BookingProcedureService _service;

        private readonly Guid _bookingProcedureId = Guid.NewGuid();
        private readonly Guid _accountId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();
        private readonly Guid _bookingItemId = Guid.NewGuid();
        private readonly Guid _bookingId = Guid.NewGuid();

        public BookingProcedureClaimStepTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingProcedureRepoMock = new Mock<IBookingProcedureRepository>();
            _nailArtistRepoMock = new Mock<INailArtistRepository>();
            _mapperMock = new Mock<IMapper>();
            _notificationServiceMock = new Mock<INotificationService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();

            _unitOfWorkMock.Setup(u => u.BookingProcedureRepository).Returns(_bookingProcedureRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_nailArtistRepoMock.Object);

            _service = new BookingProcedureService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _notificationServiceMock.Object,
                _bookingSchedulingServiceMock.Object
            );
        }

        private BookingProcedure CreateProcedure(int stepOrder = 1, BookingProcedureStatus status = BookingProcedureStatus.Pending, Guid? assignedArtistId = null)
        {
            return new BookingProcedure
            {
                BookingProcedureId = _bookingProcedureId,
                BookingItemId = _bookingItemId,
                ProcedureName = "Làm sạch móng",
                StepOrder = stepOrder,
                Status = status,
                AssignedArtistId = assignedArtistId,
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

        private NailArtist CreateArtist()
        {
            return new NailArtist
            {
                NailArtistId = _artistId,
                AccountId = _accountId,
                ConcurrentCapacity = 1,
                Account = new User
                {
                    UserId = _accountId,
                    FirstName = "Thanh",
                    LastName = "DT"
                }
            };
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID01_FirstStep_ArtistAndCustomerFree_ReturnsSuccess()
        {
            // Arrange
            var procedure = CreateProcedure(stepOrder: 1);
            var artist = CreateArtist();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync(artist);

            _bookingProcedureRepoMock.Setup(r => r.HasAnyInProgressProcedureAsync(_artistId))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.InProgress.ToString(),
                AssignedArtistId = _artistId
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Nhận công đoạn thành công. Hãy bắt đầu phục vụ.");
            result.Data.Should().NotBeNull();
            procedure.Status.Should().Be(BookingProcedureStatus.InProgress);
            procedure.AssignedArtistId.Should().Be(_artistId);
            procedure.ActualStartTime.Should().NotBeNull();
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID02_Step2_PrecedingStepCompleted_ReturnsSuccess()
        {
            // Arrange
            var step1 = new BookingProcedure
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingItemId = _bookingItemId,
                ProcedureName = "Tẩy trang móng",
                StepOrder = 1,
                Status = BookingProcedureStatus.Completed,
                IsRequired = true,
                CanOverlap = false
            };

            var step2 = CreateProcedure(stepOrder: 2);
            var artist = CreateArtist();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(step2);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync(artist);

            _bookingProcedureRepoMock.Setup(r => r.HasAnyInProgressProcedureAsync(_artistId))
                .ReturnsAsync(false);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var expectedResponse = new BookingProcedureResponseDTO
            {
                BookingProcedureId = _bookingProcedureId,
                Status = BookingProcedureStatus.InProgress.ToString()
            };
            _mapperMock.Setup(m => m.Map<BookingProcedureResponseDTO>(It.IsAny<BookingProcedure>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Nhận công đoạn thành công. Hãy bắt đầu phục vụ.");
            step2.Status.Should().Be(BookingProcedureStatus.InProgress);
            step2.AssignedArtistId.Should().Be(_artistId);
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID03_ProcedureNotFound_ReturnsError()
        {
            // Arrange
            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync((BookingProcedure?)null);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy bước quy trình yêu cầu.");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID04_ProcedureAlreadyAssignedOrNotInProgress_ReturnsError()
        {
            // Arrange
            var procedure = CreateProcedure(status: BookingProcedureStatus.InProgress, assignedArtistId: Guid.NewGuid());

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Công đoạn này đã được nhận bởi thợ khác hoặc không ở trạng thái chờ nhận.");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID05_CustomerBusyWithAnotherProcedure_ReturnsError()
        {
            // Arrange
            var targetProcedure = CreateProcedure(stepOrder: 1);
            var busyCustomerProcedure = new BookingProcedure
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingItemId = _bookingItemId,
                Status = BookingProcedureStatus.InProgress,
                ActualStartTime = DateTime.UtcNow.AddHours(7),
                ActiveDuration = 30
            };

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(targetProcedure);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { targetProcedure, busyCustomerProcedure });

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Khách hàng này đang được thực hiện một công đoạn khác. Không thể nhận thêm công đoạn lúc này.");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID06_AccountNotLinkedToArtist_ReturnsError()
        {
            // Arrange
            var procedure = CreateProcedure();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync((NailArtist?)null);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Tài khoản đăng nhập không liên kết với thợ nail nào.");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID07_ArtistBusyWithAnotherClient_ReturnsError()
        {
            // Arrange
            var procedure = CreateProcedure();
            var artist = CreateArtist();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync(artist);

            _bookingProcedureRepoMock.Setup(r => r.HasAnyInProgressProcedureAsync(_artistId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Thợ Thanh DT đang bận thực hiện công đoạn khác.");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID08_PrecedingStepNotCompleted_ReturnsError()
        {
            // Arrange
            var step1 = new BookingProcedure
            {
                BookingProcedureId = Guid.NewGuid(),
                BookingItemId = _bookingItemId,
                ProcedureName = "Tẩy trang móng",
                StepOrder = 1,
                Status = BookingProcedureStatus.Pending,
                IsRequired = true,
                CanOverlap = false
            };

            var step2 = CreateProcedure(stepOrder: 2);
            var artist = CreateArtist();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(step2);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync(artist);

            _bookingProcedureRepoMock.Setup(r => r.HasAnyInProgressProcedureAsync(_artistId))
                .ReturnsAsync(false);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingItemIdAsync(_bookingItemId))
                .ReturnsAsync(new List<BookingProcedure> { step1, step2 });

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Không thể bắt đầu bước này. Bước trước đó 'Tẩy trang móng' chưa hoàn thành");
        }

        [Fact]
        public async Task ClaimProcedureStepAsync_UTCID09_ConcurrencyExceptionOnSave_ReturnsError()
        {
            // Arrange
            var procedure = CreateProcedure();
            var artist = CreateArtist();

            _bookingProcedureRepoMock.Setup(r => r.GetProcedureWithBookingItemAsync(_bookingProcedureId, It.IsAny<bool>()))
                .ReturnsAsync(procedure);

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure> { procedure });

            _nailArtistRepoMock.Setup(r => r.GetNailArtistByAccountIdAsync(_accountId))
                .ReturnsAsync(artist);

            _bookingProcedureRepoMock.Setup(r => r.HasAnyInProgressProcedureAsync(_artistId))
                .ReturnsAsync(false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new ConcurrencyException());

            // Act
            var result = await _service.ClaimProcedureStepAsync(_bookingProcedureId, _accountId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Công đoạn này vừa được một thợ khác nhận trước đó. Vui lòng tải lại danh sách.");
        }
    }
}
