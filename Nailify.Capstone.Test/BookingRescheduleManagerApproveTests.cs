using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
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
    public class BookingRescheduleManagerApproveTests
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

        private readonly DateTime _proposedDate = DateTime.UtcNow.AddDays(2).Date;
        private readonly TimeSpan _proposedTime = new TimeSpan(14, 0, 0); // 2:00 PM

        public BookingRescheduleManagerApproveTests()
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

        private Booking CreateBooking(
            BookingStatus status = BookingStatus.ReschedulePending,
            string proposedBy = "Customer",
            bool hasProposedTime = true)
        {
            return new Booking
            {
                BookingId = _bookingId,
                CustomerId = _customerId,
                SalonId = _salonId,
                NailArtistId = _artistId,
                Status = status,
                ProposedBy = proposedBy,
                ProposedBookingDate = hasProposedTime ? _proposedDate : null,
                ProposedStartTime = hasProposedTime ? _proposedTime : null,
                BookingDate = DateTime.UtcNow.AddDays(1).Date,
                StartTime = new TimeSpan(9, 0, 0),
                TotalDuration = 60
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
                        DayOfWeek = (int)_proposedDate.DayOfWeek,
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
                WorkDate = _proposedDate,
                ShiftStart = new TimeSpan(8, 0, 0),
                ShiftEnd = new TimeSpan(17, 0, 0)
            };
            _scheduleRepoMock.Setup(r => r.GetScheduleByArtistAndDateAsync(_artistId, _proposedDate))
                .ReturnsAsync(schedule);

            _nailArtistBreakRepoMock.Setup(r => r.GetApprovedBreaksByArtistAndDateAsync(_artistId, _proposedDate))
                .ReturnsAsync(new List<NailArtistBreak>());

            _bookingProcedureRepoMock.Setup(r => r.GetProceduresByBookingIdAsync(_bookingId, It.IsAny<bool>()))
                .ReturnsAsync(new List<BookingProcedure>());

            _bookingSchedulingServiceMock.Setup(s => s.BuildProcedureTimeline(It.IsAny<List<BookingProcedure>>(), _proposedTime))
                .Returns(new List<ProcedureScheduleSegment>());

            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _proposedDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(false);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID01_ValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var booking = CreateBooking();
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(_customerId.ToString(), "BookingRescheduleApproved", It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.Approved.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Duyệt yêu cầu đổi lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Approved);
            booking.BookingDate.Should().Be(_proposedDate);
            booking.StartTime.Should().Be(_proposedTime);
            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
            _notificationServiceMock.Verify(n => n.SendNotificationToUserAsync(_customerId.ToString(), "BookingRescheduleApproved", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID02_StatusApproved_ShouldReturnError()
        {
            // Arrange
            var booking = CreateBooking(status: BookingStatus.Approved);
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không có yêu cầu đổi lịch từ khách hàng cần duyệt.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID03_ProposedByManager_ShouldReturnError()
        {
            // Arrange
            var booking = CreateBooking(proposedBy: "Manager");
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không có yêu cầu đổi lịch từ khách hàng cần duyệt.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID04_MissingProposedTime_ShouldReturnError()
        {
            // Arrange
            var booking = CreateBooking(hasProposedTime: false);
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thông tin giờ đề xuất bị thiếu.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID05_ArtistCapacityConflict_ShouldReturnError()
        {
            // Arrange
            var booking = CreateBooking();
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);
            // Override CapacityConflict to true
            _bookingSchedulingServiceMock.Setup(s => s.HasCapacityConflictAsync(_artistId, _proposedDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, null))
                .ReturnsAsync(true);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ đã kín lịch làm việc trong khung giờ này.");
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID06_NotificationServiceThrowsException_ShouldStillReturnSuccess()
        {
            // Arrange
            var booking = CreateBooking();
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _notificationServiceMock.Setup(n => n.SendNotificationToUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()))
                .ThrowsAsync(new Exception("SignalR connection error"));

            var expectedResponse = new BookingResponseDTO { BookingId = _bookingId, Status = BookingStatus.Approved.ToString() };
            _mapperMock.Setup(m => m.Map<BookingResponseDTO>(It.IsAny<Booking>()))
                .Returns(expectedResponse);

            // Act
            var result = await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Duyệt yêu cầu đổi lịch thành công.");
            booking.Status.Should().Be(BookingStatus.Approved);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ManagerApproveRescheduleAsync_UTCID07_SaveChangesAsyncThrowsException_ShouldThrowDbUpdateException()
        {
            // Arrange
            var booking = CreateBooking();
            _bookingRepoMock.Setup(r => r.GetBookingDetailAsync(_bookingId, true))
                .ReturnsAsync(booking);

            SetupValidSlot(booking);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Database connection failed"));

            // Act
            Func<Task> act = async () => await _service.ManagerApproveRescheduleAsync(_bookingId, _managerId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("*Database connection failed*");
        }
    }
}
