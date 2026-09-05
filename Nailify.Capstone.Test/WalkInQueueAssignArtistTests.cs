using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.WalkInQueueRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.WalkInQueueResponseDTOs;
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
    public class WalkInQueueAssignArtistTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWalkInQueueRepository> _walkInQueueRepoMock;
        private readonly Mock<INailArtistRepository> _nailArtistRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IScheduleRepository> _scheduleRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;

        private readonly WalkInQueueService _service;

        private readonly Guid _queueId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();
        private readonly Guid _salonId = Guid.NewGuid();
        private readonly Guid _actorId = Guid.NewGuid();

        public WalkInQueueAssignArtistTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _walkInQueueRepoMock = new Mock<IWalkInQueueRepository>();
            _nailArtistRepoMock = new Mock<INailArtistRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _scheduleRepoMock = new Mock<IScheduleRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _notificationServiceMock = new Mock<INotificationService>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();

            _unitOfWorkMock.Setup(u => u.WalkInQueueRepository).Returns(_walkInQueueRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_nailArtistRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ScheduleRepository).Returns(_scheduleRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CustomerRepository).Returns(_customerRepoMock.Object);

            _service = new WalkInQueueService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _bookingSchedulingServiceMock.Object,
                _passwordHasherMock.Object,
                _notificationServiceMock.Object,
                _bookingProcedureServiceMock.Object
            );

            SetupDefaultMapper();
        }

        private void SetupDefaultMapper()
        {
            _mapperMock.Setup(m => m.Map<WalkInQueueResponseDTO>(It.IsAny<WalkInQueue>()))
                .Returns((WalkInQueue q) => new WalkInQueueResponseDTO
                {
                    QueueId = q.QueueId,
                    SalonId = q.SalonId,
                    AssignedNailArtistId = q.AssignedNailArtistId,
                    Status = q.Status.ToString()
                });
        }

        private WalkInQueue CreateWalkInQueue()
        {
            return new WalkInQueue
            {
                QueueId = _queueId,
                SalonId = _salonId,
                Status = QueueStatus.Waiting,
                QueuePosition = 1
            };
        }

        private NailArtist CreateArtist(string status = "Active")
        {
            return new NailArtist
            {
                NailArtistId = _artistId,
                Status = status,
                ConcurrentCapacity = 1,
                Account = new User
                {
                    UserId = Guid.NewGuid(),
                    FirstName = "Thanh",
                    LastName = "DT"
                }
            };
        }

        private AssignQueueArtistRequestDTO CreateRequest()
        {
            return new AssignQueueArtistRequestDTO
            {
                NailArtistId = _artistId
            };
        }

        private void SetupCommonSchedulingDefaults(bool hasSimulationConflict = false)
        {
            _bookingSchedulingServiceMock.Setup(s => s.GenerateMockBookingProceduresAsync(It.IsAny<List<BookingItemRequestDTO>>(), _salonId))
                .ReturnsAsync(new List<BookingProcedure>());

            _bookingSchedulingServiceMock.Setup(s => s.BuildProcedureTimeline(It.IsAny<List<BookingProcedure>>(), It.IsAny<TimeSpan>()))
                .Returns(new List<ProcedureScheduleSegment>
                {
                    new ProcedureScheduleSegment
                    {
                        StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(10, 30, 0)
                    }
                });

            _bookingSchedulingServiceMock.Setup(s => s.HasSimulationConflictAsync(
                    _artistId,
                    It.IsAny<DateTime>(),
                    It.IsAny<List<ProcedureScheduleSegment>>(),
                    It.IsAny<List<ProcedureScheduleSegment>>(),
                    It.IsAny<int>(),
                    It.IsAny<Guid?>()))
                .ReturnsAsync(hasSimulationConflict);

            _bookingRepoMock.Setup(r => r.GetBookingsByArtistAndDateAsync(_artistId, It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Booking>());
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID01_ValidRequest_ShouldReturnSuccess()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var artist = CreateArtist("Active");
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(artist);

            SetupCommonSchedulingDefaults(hasSimulationConflict: false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Phân bổ thợ nail thành công.");
            result.Data.Should().NotBeNull();
            result.Data.AssignedNailArtistId.Should().Be(_artistId);
            queue.AssignedNailArtistId.Should().Be(_artistId);

            _walkInQueueRepoMock.Verify(r => r.Update(queue), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID02_QueueNotFound_ShouldReturnError()
        {
            // Arrange
            var request = CreateRequest();
            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync((WalkInQueue?)null);

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Không tìm thấy bản ghi hàng chờ.");

            _walkInQueueRepoMock.Verify(r => r.Update(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID03_ArtistNotFound_ShouldReturnError()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync((NailArtist?)null);

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ làm móng không hoạt động hoặc không tồn tại.");

            _walkInQueueRepoMock.Verify(r => r.Update(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID04_ArtistInactive_ShouldReturnError()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var artist = CreateArtist("Inactive");
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(artist);

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ làm móng không hoạt động hoặc không tồn tại.");

            _walkInQueueRepoMock.Verify(r => r.Update(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID05_SimulationConflict_ShouldReturnError()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var artist = CreateArtist("Active");
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(artist);

            SetupCommonSchedulingDefaults(hasSimulationConflict: true);

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("không có đủ thời gian trống hoặc sẽ làm trễ lịch hẹn đã đặt trước");

            _walkInQueueRepoMock.Verify(r => r.Update(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID06_DelayApprovedBooking_ShouldReturnError()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var artist = CreateArtist("Active");
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(artist);

            SetupCommonSchedulingDefaults(hasSimulationConflict: false);

            // Mock an Approved Booking that overlaps with walk-in segment (walkInSegment Start: 10:00, End: 10:30)
            var conflictingApprovedBooking = new Booking
            {
                BookingId = Guid.NewGuid(),
                NailArtistId = _artistId,
                Status = BookingStatus.Approved,
                StartTime = new TimeSpan(10, 10, 0),
                TotalDuration = 30
            };

            _bookingRepoMock.Setup(r => r.GetBookingsByArtistAndDateAsync(_artistId, It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Booking> { conflictingApprovedBooking });

            // Act
            var result = await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("không có đủ thời gian trống hoặc sẽ làm trễ lịch hẹn đã đặt trước");

            _walkInQueueRepoMock.Verify(r => r.Update(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignArtistAsync_UTCID07_SaveChangesAsyncThrowsException_ShouldThrowDbUpdateException()
        {
            // Arrange
            var queue = CreateWalkInQueue();
            var artist = CreateArtist("Active");
            var request = CreateRequest();

            _walkInQueueRepoMock.Setup(r => r.GetByIdAsync(_queueId))
                .ReturnsAsync(queue);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistWithProfileAsync(_artistId))
                .ReturnsAsync(artist);

            SetupCommonSchedulingDefaults(hasSimulationConflict: false);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("DB Write Failure"));

            // Act
            Func<Task> act = async () => await _service.AssignArtistAsync(_queueId, request, _actorId);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("*DB Write Failure*");
        }
    }
}
