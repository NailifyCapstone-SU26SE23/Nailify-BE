using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.BookingResponseDTOs;
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
    public class BookingRescheduleCustomerRequestTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<ISalonOffDateRepository> _salonOffDateRepoMock;
        private readonly Mock<ISalonRepository> _salonRepoMock;
        private readonly Mock<INailArtistRepository> _nailArtistRepoMock;
        private readonly Mock<IScheduleRepository> _scheduleRepoMock;
        private readonly Mock<INailArtistBreakRepository> _nailArtistBreakRepoMock;
        private readonly Mock<IBookingProcedureRepository> _bookingProcedureRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;

        private readonly BookingRescheduleService _service;

        private readonly Guid _bookingId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _salonId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();

        private readonly DateTime _validNewDate = DateTime.UtcNow.AddDays(2).Date;
        private readonly TimeSpan _validNewTime = new TimeSpan(10, 0, 0); // 10:00 AM

        public BookingRescheduleCustomerRequestTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _salonOffDateRepoMock = new Mock<ISalonOffDateRepository>();
            _salonRepoMock = new Mock<ISalonRepository>();
            _nailArtistRepoMock = new Mock<INailArtistRepository>();
            _scheduleRepoMock = new Mock<IScheduleRepository>();
            _nailArtistBreakRepoMock = new Mock<INailArtistBreakRepository>();
            _bookingProcedureRepoMock = new Mock<IBookingProcedureRepository>();
            _mapperMock = new Mock<IMapper>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _notificationServiceMock = new Mock<INotificationService>();

            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SalonOffDateRepository).Returns(_salonOffDateRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SalonRepository).Returns(_salonRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_nailArtistRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ScheduleRepository).Returns(_scheduleRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistBreakRepository).Returns(_nailArtistBreakRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingProcedureRepository).Returns(_bookingProcedureRepoMock.Object);

            _service = new BookingRescheduleService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _bookingSchedulingServiceMock.Object,
                _notificationServiceMock.Object
            );
        }

        private Booking CreateBooking(BookingStatus status = BookingStatus.Approved, Guid? ownerCustomerId = null)
        {
            return new Booking
            {
                BookingId = _bookingId,
                CustomerId = ownerCustomerId ?? _customerId,
                SalonId = _salonId,
                NailArtistId = _artistId,
                Status = status,
                BookingDate = DateTime.UtcNow.AddDays(1).Date,
                StartTime = new TimeSpan(9, 0, 0),
                TotalDuration = 60
            };
        }

        private CustomerRescheduleRequestDTO CreateRequest(string reason = "Work conflict")
        {
            return new CustomerRescheduleRequestDTO
            {
                NewDate = _validNewDate,
                NewTime = _validNewTime,
                Reason = reason
            };
        }

        private void SetupValidSlot(Booking booking)
        {
            _salonOffDateRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SalonOffDate, bool>>>()))
                .ReturnsAsync(false);

            var salon = new Salon
            {
                SalonId = _salonId,
                OperatingHours = new List<SalonOperatingHour>
                {
                    new SalonOperatingHour
                    {
                        DayOfWeek = (int)_validNewDate.DayOfWeek,
                        OpenTime = new TimeSpan(8, 0, 0),
                        CloseTime = new TimeSpan(20, 0, 0),
                        IsClosed = false
                    }
                }
            };
            _salonRepoMock.Setup(r => r.GetSalonWithOperatingHoursAsync(_salonId))
                .ReturnsAsync(salon);

            var artist = new NailArtist
            {
                NailArtistId = _artistId,
                Status = "Active",
                ConcurrentCapacity = 1
            };
            _nailArtistRepoMock.Setup(r => r.GetByIdAsync(_artistId))
                .ReturnsAsync(artist);

            var schedule = new Schedule
            {
                NailArtistId = _artistId,
                WorkDate = _validNewDate,
                ShiftStart = new TimeSpan(8, 0, 0),
                ShiftEnd = new TimeSpan(17, 0, 0)
            };
            _scheduleRepoMock.Setup(r => r.GetScheduleByArtistAndDateAsync(_artistId, _validNewDate))
                .ReturnsAsync(schedule);

            _nailArtistBreakRepoMock.Setup(r => r.GetApprovedBreaksByArtistAndDateAsync(_artistId, _validNewDate))
                .ReturnsAsync(new List<NailArtistBreak>());

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(new List<BookingProcedure>());

            _bookingSchedulingServiceMock.Setup(s => s.BuildProcedureTimeline(It.IsAny<List<BookingProcedure>>(), _validNewTime))
                .Returns(new List<ProcedureScheduleSegment>());

            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _validNewDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(false);
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID01_ValidSlot_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToSalonStaffAsync(_salonId.ToString(), "BookingRescheduleRequested", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ReschedulePending.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Gửi yêu cầu đổi lịch hẹn thành công.");
            booking.Status.Should().Be(BookingStatus.ReschedulePending);
            booking.ProposedBy.Should().Be("Customer");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID02_NotOwner_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(ownerCustomerId: Guid.NewGuid());
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Bạn không có quyền yêu cầu đổi lịch hẹn này.");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID03_StatusPending_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(status: BookingStatus.Pending);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Chỉ có thể đổi lịch đối với đơn đã được xác nhận (Approved).");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID04_StatusCancelled_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(status: BookingStatus.Cancelled);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Chỉ có thể đổi lịch đối với đơn đã được xác nhận (Approved).");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID05_StatusCompleted_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(status: BookingStatus.Completed);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Chỉ có thể đổi lịch đối với đơn đã được xác nhận (Approved).");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID06_SalonOffDate_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _salonOffDateRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SalonOffDate, bool>>>()))
                .ReturnsAsync(true); // Off-day

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Salon nghỉ vào ngày này. Vui lòng chọn ngày khác.");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID07_OutOfOperatingHours_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _salonOffDateRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SalonOffDate, bool>>>()))
                .ReturnsAsync(false);

            var salon = new Salon
            {
                SalonId = _salonId,
                OperatingHours = new List<SalonOperatingHour>
                {
                    new SalonOperatingHour
                    {
                        DayOfWeek = (int)_validNewDate.DayOfWeek,
                        OpenTime = new TimeSpan(8, 0, 0),
                        CloseTime = new TimeSpan(9, 0, 0), // Closed at 9am, requested 10am -> Out of operating hours
                        IsClosed = false
                    }
                }
            };
            _salonRepoMock.Setup(r => r.GetSalonWithOperatingHoursAsync(_salonId))
                .ReturnsAsync(salon);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Giờ hẹn không thuộc khung giờ hoạt động của Salon.");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID08_ArtistCapacityConflict_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _validNewDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(true); // Capacity conflict

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ đã kín lịch làm việc trong khung giờ này.");
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID09_NotificationThrowsException_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToSalonStaffAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR network drop"));

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ReschedulePending.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Gửi yêu cầu đổi lịch hẹn thành công.");
            booking.Status.Should().Be(BookingStatus.ReschedulePending);
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID10_DbUpdateException_ThrowsException()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed", new Exception()));

            // Act & Assert
            Func<Task> act = async () => await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task CustomerRequestRescheduleAsync_UTCID11_EmptyReason_Boundary_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking();
            var request = CreateRequest(reason: ""); // Boundary: empty string

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToSalonStaffAsync(_salonId.ToString(), "BookingRescheduleRequested", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.ReschedulePending.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.CustomerRequestRescheduleAsync(_bookingId, request, _customerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Gửi yêu cầu đổi lịch hẹn thành công.");
            booking.Status.Should().Be(BookingStatus.ReschedulePending);
        }
    }
}
