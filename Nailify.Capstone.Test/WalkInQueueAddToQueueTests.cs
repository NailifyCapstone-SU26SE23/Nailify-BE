using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Nailify.Capstone.Application.Common;
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
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class WalkInQueueAddToQueueTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWalkInQueueRepository> _walkInQueueRepoMock;
        private readonly Mock<IScheduleRepository> _scheduleRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<INailArtistRepository> _nailArtistRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;

        private readonly WalkInQueueService _service;

        private readonly Guid _salonId = Guid.NewGuid();
        private readonly Guid _actorId = Guid.NewGuid();
        private readonly Guid _existingCustomerId = Guid.NewGuid();

        public WalkInQueueAddToQueueTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _walkInQueueRepoMock = new Mock<IWalkInQueueRepository>();
            _scheduleRepoMock = new Mock<IScheduleRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _nailArtistRepoMock = new Mock<INailArtistRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _mapperMock = new Mock<IMapper>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _notificationServiceMock = new Mock<INotificationService>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();

            _unitOfWorkMock.Setup(u => u.WalkInQueueRepository).Returns(_walkInQueueRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ScheduleRepository).Returns(_scheduleRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.UserRepository).Returns(_userRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CustomerRepository).Returns(_customerRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_nailArtistRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);

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
            _mapperMock.Setup(m => m.Map<WalkInQueue>(It.IsAny<AddToQueueRequestDTO>()))
                .Returns((AddToQueueRequestDTO dto) => new WalkInQueue
                {
                    QueueId = Guid.NewGuid(),
                    SalonId = dto.SalonId,
                    CustomerId = dto.CustomerId,
                    GuestName = dto.GuestName,
                    GuestPhone = dto.GuestPhone,
                    RequestNote = dto.RequestNote,
                    OriginalBookingId = dto.OriginalBookingId,
                    AssignedNailArtistId = dto.AssignedNailArtistId
                });

            _mapperMock.Setup(m => m.Map<WalkInQueueResponseDTO>(It.IsAny<WalkInQueue>()))
                .Returns((WalkInQueue q) => new WalkInQueueResponseDTO
                {
                    QueueId = q.QueueId,
                    SalonId = q.SalonId,
                    CustomerId = q.CustomerId,
                    GuestName = q.GuestName,
                    GuestPhone = q.GuestPhone,
                    QueuePosition = q.QueuePosition,
                    Status = q.Status.ToString(),
                    EstimatedWait = q.EstimatedWait
                });
        }

        private AddToQueueRequestDTO CreateRequest(Guid? customerId = null, string? guestName = null, string? guestPhone = null)
        {
            return new AddToQueueRequestDTO
            {
                SalonId = _salonId,
                CustomerId = customerId,
                GuestName = guestName,
                GuestPhone = guestPhone,
                RequestNote = "Test Walk-in",
                BookingItems = new List<BookingItemRequestDTO>()
            };
        }

        private void SetupCommonDefaults(int workingArtistCount = 5, int activeWaitingCount = 1)
        {
            _scheduleRepoMock.Setup(r => r.GetWorkingArtistCountByDateAsync(_salonId, It.IsAny<DateTime>()))
                .ReturnsAsync(workingArtistCount);

            _walkInQueueRepoMock.Setup(r => r.GetActiveWaitingCountAsync(_salonId))
                .ReturnsAsync(activeWaitingCount);

            _walkInQueueRepoMock.Setup(r => r.GetNextPositionAsync(_salonId))
                .ReturnsAsync(activeWaitingCount + 1);

            _nailArtistRepoMock.Setup(r => r.GetNailArtistsBySalonIdAsync(_salonId))
                .ReturnsAsync(new List<NailArtist>());

            _walkInQueueRepoMock.Setup(r => r.GetTodayQueueAsync(_salonId, It.IsAny<bool>()))
                .ReturnsAsync(new List<WalkInQueue>());

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID01_NormalExistingCustomer_ShouldReturnSuccess()
        {
            // Arrange
            var request = CreateRequest(customerId: _existingCustomerId);
            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 1);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Contain("Đã thêm khách vào hàng chờ vãng lai");
            result.Data.Should().NotBeNull();
            result.Data.CustomerId.Should().Be(_existingCustomerId);

            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID02_NormalGuestWalkInUnregisteredPhone_ShouldAutoCreateAccountAndReturnSuccess()
        {
            // Arrange
            string guestPhone = "0912345678";
            string guestName = "Nguyen Van A";
            var request = CreateRequest(customerId: null, guestName: guestName, guestPhone: guestPhone);

            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 0);

            _userRepoMock.Setup(r => r.GetUserByPhoneAsync(guestPhone))
                .ReturnsAsync((User?)null);

            _passwordHasherMock.Setup(p => p.HashPassword("123456"))
                .Returns("hashed_password_123");

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Contain("Đã thêm khách vào hàng chờ vãng lai");
            result.Data.Should().NotBeNull();

            _userRepoMock.Verify(r => r.CreateAsync(It.Is<User>(u => u.Phone == guestPhone && u.FirstName == "Van A" && u.LastName == "Nguyen")), Times.Once);
            _customerRepoMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Once);
            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID03_AbnormalNoWorkingArtists_ShouldReturnError()
        {
            // Arrange
            var request = CreateRequest(customerId: _existingCustomerId);
            SetupCommonDefaults(workingArtistCount: 0, activeWaitingCount: 0);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Salon hôm nay không có thợ làm việc.");

            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID04_AbnormalQueueCapacityReached_ShouldReturnError()
        {
            // Arrange
            // workingArtistCount = 5 => maxWalkInCapacity = Math.Max(2, (int)Math.Ceiling(5 * 2 * 0.3)) = 3
            // activeWaitingCount = 3 (>= maxWalkInCapacity)
            var request = CreateRequest(customerId: _existingCustomerId);
            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 3);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Contain("Hàng chờ tại sảnh đã đạt giới hạn tối đa");

            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID05_NormalGuestWalkInRegisteredPhone_ShouldLinkExistingAccountAndReturnSuccess()
        {
            // Arrange
            string guestPhone = "0987654321";
            string guestName = "Tran Thi B";
            var request = CreateRequest(customerId: null, guestName: guestName, guestPhone: guestPhone);

            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 1);

            var existingUser = new User
            {
                UserId = _existingCustomerId,
                Phone = guestPhone,
                FirstName = "B",
                LastName = "Tran Thi"
            };
            var existingCustomer = new Customer
            {
                UserId = _existingCustomerId,
                LoyaltyPoint = 50
            };

            _userRepoMock.Setup(r => r.GetUserByPhoneAsync(guestPhone))
                .ReturnsAsync(existingUser);

            _customerRepoMock.Setup(r => r.GetByIdAsync(_existingCustomerId))
                .ReturnsAsync(existingCustomer);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Contain("Đã thêm khách vào hàng chờ vãng lai");
            result.Data.Should().NotBeNull();
            result.Data.CustomerId.Should().Be(_existingCustomerId);

            _userRepoMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
            _customerRepoMock.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID06_BoundaryCapacityMinusOne_ShouldReturnSuccess()
        {
            // Arrange
            // workingArtistCount = 5 => maxWalkInCapacity = 3
            // activeWaitingCount = 2 (maxWalkInCapacity - 1) -> Pass!
            var request = CreateRequest(customerId: _existingCustomerId);
            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 2);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Contain("Đã thêm khách vào hàng chờ vãng lai");
            result.Data.Should().NotBeNull();

            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID07_AbnormalDbException_ShouldThrowException()
        {
            // Arrange
            var request = CreateRequest(customerId: _existingCustomerId);
            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 0);

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("DB Write Error"));

            // Act
            Func<Task> act = async () => await _service.AddToQueueAsync(_actorId, request);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>()
                .WithMessage("*DB Write Error*");
        }

        [Fact]
        public async Task AddToQueueAsync_UTCID08_NormalCustomerIdProvided_ShouldReturnSuccess()
        {
            // Arrange
            var request = CreateRequest(customerId: _existingCustomerId);
            request.BookingItems = new List<BookingItemRequestDTO>
            {
                new BookingItemRequestDTO { ServiceId = Guid.NewGuid(), Quantity = 1 }
            };

            SetupCommonDefaults(workingArtistCount: 5, activeWaitingCount: 0);

            // Act
            var result = await _service.AddToQueueAsync(_actorId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Contain("Đã thêm khách vào hàng chờ vãng lai");
            result.Data.Should().NotBeNull();

            _walkInQueueRepoMock.Verify(r => r.CreateAsync(It.IsAny<WalkInQueue>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
        }
    }
}
