using AutoMapper;
using FluentAssertions;
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
    public class BookingLifecycleRejectBookingTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IBookingRepository> _bookingRepoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IRefundService> _refundServiceMock = new();
        private readonly BookingLifecycleService _service;

        public BookingLifecycleRejectBookingTests()
        {
            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);

            _service = new BookingLifecycleService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                Mock.Of<IWalkInQueueService>(),
                Mock.Of<IBookingSchedulingService>(),
                Mock.Of<ILoyaltyTierService>(),
                Mock.Of<ILogger<BookingService>>(),
                Mock.Of<IBookingProcedureService>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IPromotionService>(),
                Mock.Of<IOrderCodeGenerator>(),
                _refundServiceMock.Object
            );
        }

        [Fact]
        public async Task RejectBookingAsync_NoPaidTransaction_StillRejectsBooking()
        {
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var request = new RejectRequestDTO { Reason = "Salon unavailable" };
            var booking = CreatePendingBooking(bookingId);
            var responseDto = new BookingResponseDTO { BookingId = bookingId, Status = BookingStatus.Rejected.ToString() };

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(bookingId, true)).ReturnsAsync(booking);
            _refundServiceMock
                .Setup(r => r.RefundToWalletByBookingAsync(
                    bookingId,
                    "Hoàn toàn bộ tiền cọc do lịch hẹn bị từ chối.",
                    true))
                .ReturnsAsync(new PayoutResult
                {
                    Success = false,
                    Message = "Paid transaction not found for this booking"
                });
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(booking)).Returns(responseDto);

            var result = await _service.RejectBookingAsync(bookingId, actorId, request);

            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Từ chối đơn đặt lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Rejected);
            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RejectBookingAsync_RefundFails_DoesNotRejectBooking()
        {
            var bookingId = Guid.NewGuid();
            var actorId = Guid.NewGuid();
            var request = new RejectRequestDTO { Reason = "Salon unavailable" };
            var booking = CreatePendingBooking(bookingId);

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(bookingId, true)).ReturnsAsync(booking);
            _refundServiceMock
                .Setup(r => r.RefundToWalletByBookingAsync(
                    bookingId,
                    "Hoàn toàn bộ tiền cọc do lịch hẹn bị từ chối.",
                    true))
                .ReturnsAsync(new PayoutResult
                {
                    Success = false,
                    Message = "Wallet is inactive"
                });

            var result = await _service.RejectBookingAsync(bookingId, actorId, request);

            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Wallet is inactive");
            booking.Status.Should().Be(BookingStatus.Pending);
            _bookingRepoMock.Verify(r => r.Update(It.IsAny<Booking>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        private static Booking CreatePendingBooking(Guid bookingId)
            => new()
            {
                BookingId = bookingId,
                CustomerId = Guid.NewGuid(),
                SalonId = Guid.NewGuid(),
                Status = BookingStatus.Pending,
                BookingDate = DateTime.Today,
                StartTime = new TimeSpan(10, 0, 0),
                TotalDuration = 60
            };
    }
}
