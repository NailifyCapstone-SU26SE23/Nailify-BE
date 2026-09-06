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
    public class BookingRescheduleManagerSuggestTests
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
        private readonly Guid _managerId = Guid.NewGuid();
        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _salonId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();

        private readonly DateTime _validSuggestedDate = DateTime.UtcNow.AddDays(2).Date;
        private readonly TimeSpan _validSuggestedTime = new TimeSpan(14, 0, 0); // 2:00 PM

        public BookingRescheduleManagerSuggestTests()
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

        private Booking CreateBooking(BookingStatus status = BookingStatus.Approved)
        {
            return new Booking
            {
                BookingId = _bookingId,
                CustomerId = _customerId,
                SalonId = _salonId,
                NailArtistId = _artistId,
                Status = status,
                BookingDate = DateTime.UtcNow.AddDays(1).Date,
                StartTime = new TimeSpan(9, 0, 0),
                TotalDuration = 60
            };
        }

        private ManagerSuggestTimeRequestDTO CreateRequest(string reason = "Suggest new time")
        {
            return new ManagerSuggestTimeRequestDTO
            {
                SuggestedDate = _validSuggestedDate,
                SuggestedTime = _validSuggestedTime,
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
                        DayOfWeek = (int)_validSuggestedDate.DayOfWeek,
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
                WorkDate = _validSuggestedDate,
                ShiftStart = new TimeSpan(8, 0, 0),
                ShiftEnd = new TimeSpan(17, 0, 0)
            };
            _scheduleRepoMock.Setup(r => r.GetScheduleByArtistAndDateAsync(_artistId, _validSuggestedDate))
                .ReturnsAsync(schedule);

            _nailArtistBreakRepoMock.Setup(r => r.GetApprovedBreaksByArtistAndDateAsync(_artistId, _validSuggestedDate))
                .ReturnsAsync(new List<NailArtistBreak>());

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, false))
                .ReturnsAsync(new List<BookingProcedure>());

            _bookingSchedulingServiceMock.Setup(s => s.BuildProcedureTimeline(It.IsAny<List<BookingProcedure>>(), _validSuggestedTime))
                .Returns(new List<ProcedureScheduleSegment>());

            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _validSuggestedDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(false);
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID01_StatusApproved_ValidSlot_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(_customerId.ToString(), "BookingRescheduleSuggested", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.RescheduleSuggested.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Đề xuất giờ hẹn thay thế thành công.");
            booking.Status.Should().Be(BookingStatus.RescheduleSuggested);
            booking.ProposedBy.Should().Be("Manager");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID02_StatusPending_ValidSlot_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Pending);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(_customerId.ToString(), "BookingRescheduleSuggested", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.RescheduleSuggested.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Đề xuất giờ hẹn thay thế thành công.");
            booking.Status.Should().Be(BookingStatus.RescheduleSuggested);
            booking.ProposedBy.Should().Be("Manager");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID03_StatusCancelled_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Cancelled);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không thể đề xuất giờ mới cho đơn hàng đã hoàn tất, đã check-in hoặc đã bị hủy.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID04_StatusCompleted_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Completed);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không thể đề xuất giờ mới cho đơn hàng đã hoàn tất, đã check-in hoặc đã bị hủy.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID05_StatusCheckedIn_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.CheckedIn);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không thể đề xuất giờ mới cho đơn hàng đã hoàn tất, đã check-in hoặc đã bị hủy.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID06_StatusInProgress_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.InProgress);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không thể đề xuất giờ mới cho đơn hàng đã hoàn tất, đã check-in hoặc đã bị hủy.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID07_SalonOffDate_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            _salonOffDateRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SalonOffDate, bool>>>()))
                .ReturnsAsync(true); // Salon off-day

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Salon nghỉ vào ngày này. Vui lòng chọn ngày khác.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID08_OutOfOperatingHours_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
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
                        DayOfWeek = (int)_validSuggestedDate.DayOfWeek,
                        OpenTime = new TimeSpan(8, 0, 0),
                        CloseTime = new TimeSpan(12, 0, 0), // Closed at 12pm, requested 14:00 (2pm)
                        IsClosed = false
                    }
                }
            };
            _salonRepoMock.Setup(r => r.GetSalonWithOperatingHoursAsync(_salonId))
                .ReturnsAsync(salon);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Giờ hẹn không thuộc khung giờ hoạt động của Salon.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID09_ArtistCapacityConflict_ReturnsError()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _validSuggestedDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(true); // Capacity conflict

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ đã kín lịch làm việc trong khung giờ này.");
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID10_NotificationThrowsException_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR connection error"));

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.RescheduleSuggested.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Đề xuất giờ hẹn thay thế thành công.");
            booking.Status.Should().Be(BookingStatus.RescheduleSuggested);
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID11_DbUpdateException_ThrowsException()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest();

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed", new Exception()));

            // Act & Assert
            Func<Task> act = async () => await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task ManagerSuggestTimeAsync_UTCID12_EmptyReason_Boundary_ReturnsSuccess()
        {
            // Arrange
            var booking = CreateBooking(BookingStatus.Approved);
            var request = CreateRequest(reason: ""); // Boundary empty string

            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(_customerId.ToString(), "BookingRescheduleSuggested", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.RescheduleSuggested.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerSuggestTimeAsync(_bookingId, request, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Đề xuất giờ hẹn thay thế thành công.");
            booking.Status.Should().Be(BookingStatus.RescheduleSuggested);
        }
    }
}
