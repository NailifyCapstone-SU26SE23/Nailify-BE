using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.Common.Models.Scheduling;
using Nailify.Capstone.Application.DTOs.RequestDTOs.BookingRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Application.Services;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class BookingCreationCreateBookingTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<ISalonRepository> _salonRepoMock;
        private readonly Mock<INailArtistRepository> _artistRepoMock;
        private readonly Mock<INailArtistBreakRepository> _artistBreakRepoMock;
        private readonly Mock<ISalonOffDateRepository> _salonOffDateRepoMock;
        private readonly Mock<ICustomerNailRequestRepository> _customerNailRequestRepoMock;
        private readonly Mock<ICustomerNailRepository> _customerNailRepoMock;
        private readonly Mock<IBookingProcedureRepository> _bookingProcedureRepoMock;
        private readonly Mock<IShapeMethodConfigRepository> _shapeMethodConfigRepoMock;

        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IQRService> _qrServiceMock;
        private readonly Mock<IBookingProcedureService> _bookingProcedureServiceMock;
        private readonly Mock<ILoyaltyTierService> _loyaltyTierServiceMock;
        private readonly Mock<ISlotHoldService> _slotHoldServiceMock;
        private readonly Mock<IPromotionService> _promotionServiceMock;
        private readonly Mock<IBookingSchedulingService> _bookingSchedulingServiceMock;
        private readonly Mock<INailVariantService> _nailVariantServiceMock;
        private readonly Mock<ILogger<BookingCreationService>> _loggerMock;

        private readonly BookingCreationService _service;

        private readonly Guid _customerId = Guid.NewGuid();
        private readonly Guid _salonId = Guid.NewGuid();
        private readonly Guid _artistId = Guid.NewGuid();
        private readonly int _variantId = 17;
        private readonly DateTime _bookingDate = DateTime.UtcNow.AddHours(7).Date.AddDays(1); // Future date
        private readonly TimeSpan _startTime = new TimeSpan(10, 0, 0);

        public BookingCreationCreateBookingTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _bookingRepoMock = new Mock<IBookingRepository>();
            _salonRepoMock = new Mock<ISalonRepository>();
            _artistRepoMock = new Mock<INailArtistRepository>();
            _artistBreakRepoMock = new Mock<INailArtistBreakRepository>();
            _salonOffDateRepoMock = new Mock<ISalonOffDateRepository>();
            _customerNailRequestRepoMock = new Mock<ICustomerNailRequestRepository>();
            _customerNailRepoMock = new Mock<ICustomerNailRepository>();
            _bookingProcedureRepoMock = new Mock<IBookingProcedureRepository>();
            _shapeMethodConfigRepoMock = new Mock<IShapeMethodConfigRepository>();

            _mapperMock = new Mock<IMapper>();
            _qrServiceMock = new Mock<IQRService>();
            _bookingProcedureServiceMock = new Mock<IBookingProcedureService>();
            _loyaltyTierServiceMock = new Mock<ILoyaltyTierService>();
            _slotHoldServiceMock = new Mock<ISlotHoldService>();
            _promotionServiceMock = new Mock<IPromotionService>();
            _bookingSchedulingServiceMock = new Mock<IBookingSchedulingService>();
            _nailVariantServiceMock = new Mock<INailVariantService>();
            _loggerMock = new Mock<ILogger<BookingCreationService>>();

            // Setup sub-repositories on UnitOfWork
            _unitOfWorkMock.Setup(u => u.BookingRepository).Returns(_bookingRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SalonRepository).Returns(_salonRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistRepository).Returns(_artistRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.NailArtistBreakRepository).Returns(_artistBreakRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.SalonOffDateRepository).Returns(_salonOffDateRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CustomerNailRequestRepository).Returns(_customerNailRequestRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.CustomerNailRepository).Returns(_customerNailRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.BookingProcedureRepository).Returns(_bookingProcedureRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.ShapeMethodConfigRepository).Returns(_shapeMethodConfigRepoMock.Object);

            _service = new BookingCreationService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _qrServiceMock.Object,
                _bookingProcedureServiceMock.Object,
                _loyaltyTierServiceMock.Object,
                _slotHoldServiceMock.Object,
                _promotionServiceMock.Object,
                _bookingSchedulingServiceMock.Object,
                _nailVariantServiceMock.Object,
                _loggerMock.Object
            );
        }

        private CreateBookingRequestDTO CreateStandardRequest(string? holdToken = null)
        {
            return new CreateBookingRequestDTO
            {
                SalonId = _salonId,
                NailArtistId = _artistId,
                BookingDate = _bookingDate,
                StartTime = _startTime,
                HoldToken = holdToken,
                BookingItems = new List<BookingItemRequestDTO>
                {
                    new BookingItemRequestDTO
                    {
                        NailVariantId = _variantId,
                        Quantity = 1
                    }
                }
            };
        }

        private void SetupCommonMocks(bool isWithinOperatingHours = true, string artistStatus = "Active")
        {
            // Salon Off Date: false
            _salonOffDateRepoMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<SalonOffDate, bool>>>()))
                .ReturnsAsync(false);

            // Mapping Request Items -> BookingItems
            _mapperMock
                .Setup(m => m.Map<List<BookingItem>>(It.IsAny<IEnumerable<BookingItemRequestDTO>>()))
                .Returns((IEnumerable<BookingItemRequestDTO> src) => src.Select(i => new BookingItem
                {
                    NailVariantId = i.NailVariantId,
                    Quantity = i.Quantity
                }).ToList());

            // Nail Variant details
            _nailVariantServiceMock
                .Setup(s => s.GetNailVariantByIdAsync(_variantId, It.IsAny<Guid?>()))
                .ReturnsAsync(new ApiSuccessResult<NailVariantDto>(new NailVariantDto
                {
                    NailVariantId = _variantId,
                    Price = 100000,
                    Duration = 60
                }));

            // Loyalty Tier
            _loyaltyTierServiceMock
                .Setup(s => s.GetMyLoyaltyAsync(_customerId))
                .ReturnsAsync(new ApiSuccessResult<UserLoyaltyDto>(new UserLoyaltyDto
                {
                    LoyaltyTier = new LoyaltyTierDto
                    {
                        Name = "Bronze",
                        DiscountRate = 0
                    }
                }));

            // Promotions
            _promotionServiceMock
                .Setup(s => s.GetApplicablePromotionsAsync(_customerId, It.IsAny<ICollection<BookingItem>>(), It.IsAny<IEnumerable<int>?>()))
                .ReturnsAsync(new List<Promotion>());

            _promotionServiceMock
                .Setup(s => s.CalculateDiscountsAsync(It.IsAny<Booking>(), It.IsAny<List<Promotion>>()))
                .ReturnsAsync((0m, new List<BookingDiscount>()));

            // QR Code
            _qrServiceMock
                .Setup(q => q.GenerateQRCode(It.IsAny<string>()))
                .Returns("QR_BASE64_CODE");

            // Mapping Request -> Booking entity
            _mapperMock
                .Setup(m => m.Map<Booking>(It.IsAny<CreateBookingRequestDTO>()))
                .Returns((CreateBookingRequestDTO req) => new Booking
                {
                    SalonId = req.SalonId,
                    NailArtistId = req.NailArtistId,
                    BookingDate = req.BookingDate,
                    StartTime = req.StartTime
                });

            // Operating Hours
            int dayOfWeek = (int)_bookingDate.DayOfWeek;
            var operatingHours = new List<SalonOperatingHour>
            {
                new SalonOperatingHour
                {
                    DayOfWeek = dayOfWeek,
                    OpenTime = isWithinOperatingHours ? new TimeSpan(8, 0, 0) : new TimeSpan(18, 0, 0),
                    CloseTime = isWithinOperatingHours ? new TimeSpan(20, 0, 0) : new TimeSpan(22, 0, 0),
                    IsClosed = false
                }
            };
            _salonRepoMock
                .Setup(r => r.GetSalonWithOperatingHoursAsync(_salonId))
                .ReturnsAsync(new Salon { SalonId = _salonId, OperatingHours = operatingHours });

            // Artist Breaks
            _artistBreakRepoMock
                .Setup(r => r.GetApprovedBreaksByArtistAndDateAsync(_artistId, _bookingDate))
                .ReturnsAsync(new List<NailArtistBreak>());

            // Lock Artist
            if (artistStatus != null)
            {
                _artistRepoMock
                    .Setup(r => r.GetArtistWithLockAsync(_artistId))
                    .ReturnsAsync(new NailArtist { NailArtistId = _artistId, Status = artistStatus, ConcurrentCapacity = 1 });
            }
            else
            {
                _artistRepoMock
                    .Setup(r => r.GetArtistWithLockAsync(_artistId))
                    .ReturnsAsync((NailArtist?)null);
            }

            // Booking procedures timeline mocks
            _bookingSchedulingServiceMock
                .Setup(s => s.GenerateMockBookingProceduresAsync(It.IsAny<List<BookingItemRequestDTO>>(), _salonId))
                .ReturnsAsync(new List<BookingProcedure>());

            _bookingSchedulingServiceMock
                .Setup(s => s.BuildProcedureTimeline(It.IsAny<List<BookingProcedure>>(), _startTime))
                .Returns(new List<ProcedureScheduleSegment>());

            // Procedures repo
            _bookingProcedureRepoMock
                .Setup(r => r.GetProceduresByBookingIdAsync(It.IsAny<Guid>(), true))
                .ReturnsAsync(new List<BookingProcedure>());
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID01_ValidHoldToken_ReturnsSuccess()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: "HOLD_123");
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Active");

            _slotHoldServiceMock
                .Setup(s => s.ValidateHoldTokenAsync("HOLD_123", _customerId, _artistId, _bookingDate, _startTime))
                .ReturnsAsync(true);

            _bookingSchedulingServiceMock
                .Setup(s => s.HasCapacityConflictAsync(_artistId, _bookingDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Tạo đơn đặt lịch thành công.");
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
            _slotHoldServiceMock.Verify(s => s.ConsumeHoldAsync("HOLD_123"), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID02_NullHoldToken_SlotNotHeld_ReturnsSuccess()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: null);
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Active");

            _slotHoldServiceMock
                .Setup(s => s.IsSlotHeldAsync(_artistId, _bookingDate, _startTime, It.IsAny<TimeSpan>()))
                .ReturnsAsync(false);

            _bookingSchedulingServiceMock
                .Setup(s => s.HasCapacityConflictAsync(_artistId, _bookingDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, It.IsAny<Guid?>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeTrue();
            result.Message.Should().Be("Tạo đơn đặt lịch thành công.");
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID03_InvalidOrExpiredHoldToken_ReturnsErrorAndRollback()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: "EXPIRED_999");
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Active");

            _slotHoldServiceMock
                .Setup(s => s.ValidateHoldTokenAsync("EXPIRED_999", _customerId, _artistId, _bookingDate, _startTime))
                .ReturnsAsync(false);

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Mã giữ chỗ không hợp lệ hoặc đã hết hạn.");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID04_NullHoldToken_SlotIsHeld_ReturnsErrorAndRollback()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: null);
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Active");

            _slotHoldServiceMock
                .Setup(s => s.IsSlotHeldAsync(_artistId, _bookingDate, _startTime, It.IsAny<TimeSpan>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Khoảng thời gian này đang có người giữ chỗ. Vui lòng chọn giờ khác.");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID05_NullHoldToken_CapacityConflict_ReturnsErrorAndRollback()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: null);
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Active");

            _slotHoldServiceMock
                .Setup(s => s.IsSlotHeldAsync(_artistId, _bookingDate, _startTime, It.IsAny<TimeSpan>()))
                .ReturnsAsync(false);

            _bookingSchedulingServiceMock
                .Setup(s => s.HasCapacityConflictAsync(_artistId, _bookingDate, It.IsAny<List<ProcedureScheduleSegment>>(), 1, It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Khoảng thời gian này thợ đã bận, xin chọn giờ khác.");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID06_OutsideOperatingHours_ReturnsErrorAndRollback()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: null);
            SetupCommonMocks(isWithinOperatingHours: false, artistStatus: "Active");

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thời gian đặt lịch không nằm trong giờ hoạt động của Salon.");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID07_ArtistInactiveOrNull_ReturnsErrorAndRollback()
        {
            // Arrange
            var request = CreateStandardRequest(holdToken: null);
            SetupCommonMocks(isWithinOperatingHours: true, artistStatus: "Inactive");

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Thợ nail không hoạt động hoặc không tồn tại.");
            _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_UTCID08_EmptyBookingItems_ReturnsError()
        {
            // Arrange
            var request = new CreateBookingRequestDTO
            {
                SalonId = _salonId,
                NailArtistId = _artistId,
                BookingDate = _bookingDate,
                StartTime = _startTime,
                BookingItems = new List<BookingItemRequestDTO>()
            };

            // Act
            var result = await _service.CreateBookingAsync(_customerId, request);

            // Assert
            result.IsSucceeded.Should().BeFalse();
            result.Message.Should().Be("Vui lòng chọn ít nhất một mẫu móng hoặc dịch vụ.");
            _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        }
    }
}
