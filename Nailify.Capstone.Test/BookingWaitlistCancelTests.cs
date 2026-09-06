using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WaitlistResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Common.Events.BookingEvents;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingWaitlistCancelTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingWaitlistRepository> _waitlistRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILoyaltyTierService> _loyaltyTierServiceMock;
        private readonly Mock<IPromotionService> _promotionServiceMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;

        private readonly BookingWaitlistService _service;

        private readonly Guid _waitlistId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _salonId = Guid.NewGuid();

        public BookingWaitlistCancelTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _waitlistRepoMock = new Mock<IBookingWaitlistRepository>();
            _mapperMock = new Mock<IMapper>();
            _loyaltyTierServiceMock = new Mock<ILoyaltyTierService>();
            _promotionServiceMock = new Mock<IPromotionService>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();

            _unitOfWorkMock.Setup(u => u.BookingWaitlistRepository).Returns(_waitlistRepoMock.Object);

            _service = new BookingWaitlistService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _loyaltyTierServiceMock.Object,
                _promotionServiceMock.Object,
                _bookingProcedureServiceMock.Object,
                _bookingSchedulingServiceMock.Object
            );

            SetupDefaultMapper();
        }

        private void SetupDefaultMapper()
        {
            _mapperMock.Setup(m => m.Map<WaitlistResponseDTO>(It.IsAny<BookingWaitlist>()))
                .Returns((BookingWaitlist w) => new WaitlistResponseDTO
                {
                    WailistId = w.WailistId,
                    CustomerId = w.CustomerId,
                    SalonId = w.SalonId,
                    Status = w.Status.ToString()
                });
        }

        private BookingWaitlist CreateWaitlist(WaitlistStatus status = WaitlistStatus.Waiting, Guid? ownerCustomerId = null)
        {
            return new BookingWaitlist
            {
                WailistId = _waitlistId,
                CustomerId = ownerCustomerId ?? _customerId,
                SalonId = _salonId,
                Status = status,
                RequestedDate = DateTime.UtcNow.AddDays(1).Date,
                RequestedStartTime = new TimeSpan(14, 0, 0)
            };
        }

        [Fact]
        public async Task CancelWaitlistAsync_UTCID01_NormalPreviousStatusWaiting_ShouldReturnSuccessAndNoDomainEvent()
        {
            // Arrange
            var waitlist = CreateWaitlist(status: WaitlistStatus.Waiting);

            _waitlistRepoMock.Setup(r => r.GetByIdAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _waitlistRepoMock.Setup(r => r.GetWaitlistWithDetailsAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CancelWaitlistAsync(_waitlistId, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy vị trí trong hàng chờ thành công.");
            result.Data.Should().NotBeNull();
            waitlist.Status.Should().Be(WaitlistStatus.Cancelled);
            waitlist.GetDomainEvents().Should().BeEmpty();

            _waitlistRepoMock.Verify(r => r.Update(waitlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelWaitlistAsync_UTCID02_NormalPreviousStatusNotified_ShouldReturnSuccessAndAddSlotFreedEvent()
        {
            // Arrange
            var waitlist = CreateWaitlist(status: WaitlistStatus.Notified);

            _waitlistRepoMock.Setup(r => r.GetByIdAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _waitlistRepoMock.Setup(r => r.GetWaitlistWithDetailsAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CancelWaitlistAsync(_waitlistId, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy vị trí trong hàng chờ thành công.");
            waitlist.Status.Should().Be(WaitlistStatus.Cancelled);

            var domainEvents = waitlist.GetDomainEvents();
            domainEvents.Should().HaveCount(1);
            domainEvents.First().Should().BeOfType<SlotFreedEvent>();

            var freedEvent = (SlotFreedEvent)domainEvents.First();
            freedEvent.SalonId.Should().Be(_salonId);
            freedEvent.BookingDate.Should().Be(waitlist.RequestedDate.Date);
            freedEvent.StartTime.Should().Be(waitlist.RequestedStartTime);

            _waitlistRepoMock.Verify(r => r.Update(waitlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelWaitlistAsync_UTCID03_AbnormalNotFoundOrOwnershipMismatch_ShouldReturnError()
        {
            // Arrange
            Guid wrongCustomerId = Guid.NewGuid();
            var waitlist = CreateWaitlist(status: WaitlistStatus.Waiting, ownerCustomerId: wrongCustomerId);

            _waitlistRepoMock.Setup(r => r.GetByIdAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            // Act
            var result = await _service.CancelWaitlistAsync(_waitlistId, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy thông tin hàng chờ.");

            _waitlistRepoMock.Verify(r => r.Update(It.IsAny<BookingWaitlist>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelWaitlistAsync_UTCID04_AbnormalDbException_ShouldThrowDbUpdateException()
        {
            // Arrange
            var waitlist = CreateWaitlist(status: WaitlistStatus.Waiting);

            _waitlistRepoMock.Setup(r => r.GetByIdAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection lost"));

            // Act
            Func<Task> act = async () => await _service.CancelWaitlistAsync(_waitlistId, _customerId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("*Database connection lost*");
        }

        [Fact]
        public async Task CancelWaitlistAsync_UTCID05_BoundaryPreviousStatusCancelled_ShouldReturnSuccess()
        {
            // Arrange
            var waitlist = CreateWaitlist(status: WaitlistStatus.Cancelled);

            _waitlistRepoMock.Setup(r => r.GetByIdAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _waitlistRepoMock.Setup(r => r.GetWaitlistWithDetailsAsync(_waitlistId))
                .ReturnsAsync(waitlist);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CancelWaitlistAsync(_waitlistId, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Hủy vị trí trong hàng chờ thành công.");
            waitlist.Status.Should().Be(WaitlistStatus.Cancelled);
            waitlist.GetDomainEvents().Should().BeEmpty();

            _waitlistRepoMock.Verify(r => r.Update(waitlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
