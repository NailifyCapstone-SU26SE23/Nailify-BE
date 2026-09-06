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
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingLifecycleCancelBookingTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IWalkInQueueService> _queueServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<ILoyaltyTierService> _loyaltyTierServiceMock;
        private readonly Mock<ILogger<BookingService>> _loggerMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IPromotionService> _promotionServiceMock;
        private readonly Mock<IOrderCodeGenerator> _orderCodeGeneratorMock;

        private readonly BookingLifecycleService _service;

        public BookingLifecycleCancelBookingTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _mapperMock = new Mock<IMapper>();
            _queueServiceMock = new Mock<IWalkInQueueService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _loyaltyTierServiceMock = new Mock<ILoyaltyTierService>();
            _loggerMock = new Mock<ILogger<BookingService>>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _promotionServiceMock = new Mock<IPromotionService>();
            _orderCodeGeneratorMock = new Mock<IOrderCodeGenerator>();

            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);

            _service = new BookingLifecycleService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _queueServiceMock.Object,
                _bookingSchedulingServiceMock.Object,
                _loyaltyTierServiceMock.Object,
                _loggerMock.Object,
                _bookingProcedureServiceMock.Object,
                _notificationServiceMock.Object,
                _promotionServiceMock.Object,
                _orderCodeGeneratorMock.Object
            );
        }

        private Booking CreateSampleBooking(Guid bookingId, Guid customerId, BookingStatus status)
        {
            return new Booking
            {
                BookingId = bookingId,
                CustomerId = customerId,
                SalonId = Guid.NewGuid(),
                Status = status,
                BookingDate = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0)
            };
        }

        // UTCID01 - Valid bookingId & Pending status -> Returns ApiSuccessResult (IsSucceeded = true), status set to Cancelled
        [Fact]
        public async Task CancelBookingAsync_UTCID01_PendingStatus_ReturnsApiSuccessResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Pending);
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Cancelled.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy đơn đặt lịch thành công.");
            result.Data.Should().NotBeNull();
            booking.Status.Should().Be(BookingStatus.Cancelled);

            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationToUserAsync(
                customerId.ToString(),
                "BookingRejected",
                It.IsAny<object>()
            ), Times.Once);
        }

        // UTCID02 - Valid bookingId & Approved status -> Returns ApiSuccessResult (IsSucceeded = true)
        [Fact]
        public async Task CancelBookingAsync_UTCID02_ApprovedStatus_ReturnsApiSuccessResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Approved);
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Cancelled.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Cancelled);

            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // UTCID03 - Non-existent bookingId (null entity) -> Returns ApiErrorResult ("Đơn đặt lịch không tồn tại.")
        [Fact]
        public async Task CancelBookingAsync_UTCID03_NonExistentBooking_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Đơn đặt lịch không tồn tại.");

            _bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
            _notificationServiceMock.Verify(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        // UTCID04 - Booking Status = CheckedIn -> Returns ApiErrorResult
        [Fact]
        public async Task CancelBookingAsync_UTCID04_CheckedInStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.CheckedIn);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'");
            booking.Status.Should().Be(BookingStatus.CheckedIn);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID05 - Booking Status = InProgress -> Returns ApiErrorResult
        [Fact]
        public async Task CancelBookingAsync_UTCID05_InProgressStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.InProgress);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'");
            booking.Status.Should().Be(BookingStatus.InProgress);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID06 - Booking Status = Completed -> Returns ApiErrorResult
        [Fact]
        public async Task CancelBookingAsync_UTCID06_CompletedStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Completed);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'");
            booking.Status.Should().Be(BookingStatus.Completed);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID07 - Booking Status = Cancelled -> Returns ApiErrorResult
        [Fact]
        public async Task CancelBookingAsync_UTCID07_AlreadyCancelledStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Cancelled);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ được hủy đơn ở trạng thái 'Pending' hoặc 'Approved'");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID08 - SaveChangesAsync Throws Exception -> Throws DbUpdateException
        [Fact]
        public async Task CancelBookingAsync_UTCID08_DatabaseSaveException_ThrowsException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Pending);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed", new Exception("DB Timeout")));

            // Act
            Func<Task> act = async () => await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("Database connection failed");
        }

        // UTCID09 - Notification Throws Exception -> Internal try-catch catches error and cancellation still succeeds
        [Fact]
        public async Task CancelBookingAsync_UTCID09_NotificationServiceThrowsException_CancellationStillSucceeds()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "Personal emergency" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Pending);
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Cancelled.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            _notificationServiceMock
                .Setup(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR connection dropped"));

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Cancelled);

            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // UTCID10 - Input Reason is Empty String ("") -> Cancellation succeeds
        [Fact]
        public async Task CancelBookingAsync_UTCID10_EmptyReason_ReturnsApiSuccessResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var request = new CancelBookingRequestDTO { Reason = "" };

            var booking = CreateSampleBooking(bookingId, customerId, BookingStatus.Pending);
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Cancelled.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            // Act
            var result = await _service.CancelBookingAsync(bookingId, customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Cancelled);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
