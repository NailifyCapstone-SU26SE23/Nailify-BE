using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingLifecycleStartServiceTests
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

        private readonly BookingLifecycleService _service;

        private readonly Guid _bookingId = Guid.NewGuid();
        private readonly Guid _actorId = Guid.NewGuid();

        public BookingLifecycleStartServiceTests()
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
                _promotionServiceMock.Object
            );
        }

        [Fact]
        public async Task StartServiceAsync_UTCID01_CheckedIn_ReturnsSuccess()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.CheckedIn
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _mapperMock
                .Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.InProgress.ToString() });

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Bắt đầu làm móng thành công.");
            booking.Status.Should().Be(BookingStatus.InProgress);
            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID02_BookingNotFound_ReturnsError()
        {
            // Arrange
            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Đơn đặt lịch không tồn tại.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID03_StatusPending_ReturnsError()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.Pending
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID04_StatusApproved_ReturnsError()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.Approved
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID05_StatusInProgress_ReturnsError()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.InProgress
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID06_StatusCompletedOrCancelled_ReturnsError()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.Completed
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Chỉ có thể bắt đầu làm khi khách đã 'CheckedIn'");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task StartServiceAsync_UTCID07_DbException_ThrowsException()
        {
            // Arrange
            var booking = new Booking
            {
                BookingId = _bookingId,
                Status = BookingStatus.CheckedIn
            };

            _bookingRepoMock
                .Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _unitOfWorkMock
                .Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.StartServiceAsync(_bookingId, _actorId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("Database connection failed");
        }
    }
}
