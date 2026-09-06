using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingLifecycleConfirmBookingTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IChairRepository> _chairRepoMock;
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

        public BookingLifecycleConfirmBookingTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _chairRepoMock = new Mock<IChairRepository>();
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
            _unitOfWorkMock.Setup(u => u.ChairRepository).Returns(_chairRepoMock.Object);
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

        private Booking CreateSampleBooking(Guid bookingId, BookingStatus status, Guid salonId, Guid? nailArtistId = null)
        {
            return new Booking
            {
                BookingId = bookingId,
                CustomerId = Guid.NewGuid(),
                SalonId = salonId,
                NailArtistId = nailArtistId ?? Guid.NewGuid(),
                Status = status,
                BookingDate = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0),
                TotalDuration = 60
            };
        }

        // UTCID01 - Valid bookingId & Pending status & Available Chairs -> Returns ApiSuccessResult (IsSucceeded = true)
        [Fact]
        public async Task ConfirmBookingAsync_UTCID01_PendingStatusAndAvailableChairs_ReturnsApiSuccessResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Pending, salonId);

            var activeChairs = new List<Chair> { new Chair { ChairId = Guid.NewGuid(), SalonId = salonId, Status = "Active" } };
            var procedures = new List<BookingProcedure>
            {
                new BookingProcedure
                {
                    BookingProcedureId = Guid.NewGuid(),
                    BookingItemId = Guid.NewGuid(),
                    ActiveDuration = 30,
                    IsMainStep = true
                }
            };
            var timeline = new List<ProcedureScheduleSegment>
            {
                new ProcedureScheduleSegment
                {
                    BookingProcedureId = procedures[0].BookingProcedureId,
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(10, 30, 0)
                }
            };
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Approved.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _chairRepoMock
                .Setup(c => c.GetActiveChairsBySalonAsync(salonId))
                .ReturnsAsync(activeChairs);

            _bookingRepoMock
                .Setup(r => r.CountApprovedOverlappingAsync(salonId, booking.BookingDate, booking.StartTime, booking.TotalDuration, bookingId))
                .ReturnsAsync(0);

            _bookingProcedureRepoMock
                .Setup(p => p.GetProceduresByBookingIdAsync(bookingId, true))
                .ReturnsAsync(procedures);

            _bookingSchedulingServiceMock
                .Setup(s => s.BuildProcedureTimeline(procedures, booking.StartTime))
                .Returns(timeline);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Duyệt đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Approved);

            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationToUserAsync(
                booking.CustomerId.ToString(),
                "BookingConfirmed",
                It.IsAny<object>()
            ), Times.Once);
        }

        // UTCID02 - Non-existent bookingId (null entity) -> Returns ApiErrorResult ("Đơn đặt lịch không tồn tại.")
        [Fact]
        public async Task ConfirmBookingAsync_UTCID02_NonExistentBooking_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Đơn đặt lịch không tồn tại.");

            _bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID03 - Booking Status = Approved -> Returns ApiErrorResult
        [Fact]
        public async Task ConfirmBookingAsync_UTCID03_AlreadyApprovedStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Approved, salonId);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể xác nhận đơn ở trạng thái 'Pending'");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // ✅ UTCID04 - Booking Status = Cancelled -> Returns ApiErrorResult
        [Fact]
        public async Task ConfirmBookingAsync_UTCID04_CancelledStatus_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Cancelled, salonId);

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể xác nhận đơn ở trạng thái 'Pending'");

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID05 - Salon Full (approvedOverlapCount >= activeChairCount) -> Returns ApiErrorResult
        [Fact]
        public async Task ConfirmBookingAsync_UTCID05_SalonFullCapacity_ReturnsApiErrorResult()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Pending, salonId);

            var activeChairs = new List<Chair>
            {
                new Chair { ChairId = Guid.NewGuid(), SalonId = salonId, Status = "Active" },
                new Chair { ChairId = Guid.NewGuid(), SalonId = salonId, Status = "Active" }
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _chairRepoMock
                .Setup(c => c.GetActiveChairsBySalonAsync(salonId))
                .ReturnsAsync(activeChairs);

            // 2 chairs active, 2 overlap bookings approved -> Full capacity!
            _bookingRepoMock
                .Setup(r => r.CountApprovedOverlappingAsync(salonId, booking.BookingDate, booking.StartTime, booking.TotalDuration, bookingId))
                .ReturnsAsync(2);

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Không thể duyệt: Salon đã có 2/2 ghế được đặt trong khung giờ này.");

            _bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        // UTCID06 - Notification Throws Exception -> Fault Tolerance, confirmation still succeeds
        [Fact]
        public async Task ConfirmBookingAsync_UTCID06_NotificationServiceThrowsException_ConfirmationStillSucceeds()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Pending, salonId);

            var activeChairs = new List<Chair> { new Chair { ChairId = Guid.NewGuid(), SalonId = salonId, Status = "Active" } };
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Approved.ToString() };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _chairRepoMock
                .Setup(c => c.GetActiveChairsBySalonAsync(salonId))
                .ReturnsAsync(activeChairs);

            _bookingRepoMock
                .Setup(r => r.CountApprovedOverlappingAsync(salonId, booking.BookingDate, booking.StartTime, booking.TotalDuration, bookingId))
                .ReturnsAsync(0);

            _bookingProcedureRepoMock
                .Setup(p => p.GetProceduresByBookingIdAsync(bookingId, true))
                .ReturnsAsync(new List<BookingProcedure>());

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(booking))
                .Returns(responseDto);

            _notificationServiceMock
                .Setup(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR network drop"));

            // Act
            var result = await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Duyệt đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Approved);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // UTCID07 - Database SaveChangesAsync Throws Exception -> Throws DbUpdateException
        [Fact]
        public async Task ConfirmBookingAsync_UTCID07_DatabaseSaveException_ThrowsException()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var salonId = Guid.NewGuid();
            var booking = CreateSampleBooking(bookingId, BookingStatus.Pending, salonId);

            var activeChairs = new List<Chair> { new Chair { ChairId = Guid.NewGuid(), SalonId = salonId, Status = "Active" } };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(bookingId, true))
                .ReturnsAsync(booking);

            _chairRepoMock
                .Setup(c => c.GetActiveChairsBySalonAsync(salonId))
                .ReturnsAsync(activeChairs);

            _bookingRepoMock
                .Setup(r => r.CountApprovedOverlappingAsync(salonId, booking.BookingDate, booking.StartTime, booking.TotalDuration, bookingId))
                .ReturnsAsync(0);

            _bookingProcedureRepoMock
                .Setup(p => p.GetProceduresByBookingIdAsync(bookingId, true))
                .ReturnsAsync(new List<BookingProcedure>());

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed", new Exception("DB Connection Refused")));

            // Act
            Func<Task> act = async () => await _service.ConfirmBookingAsync(bookingId, actorId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("Database connection failed");
        }
    }
}
