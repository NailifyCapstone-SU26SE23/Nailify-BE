using Microsoft.AspNetCore.Mvc;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistRequestDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Presentation.Controllers;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class ArtistEmergencyOffTest
    {
        private readonly Mock<INailArtistEmergencyService> _emergencyServiceMock;
        private readonly EmergencyOffController _controller;

        public ArtistEmergencyOffTest()
        {
            _emergencyServiceMock = new Mock<INailArtistEmergencyService>();
            _controller = new EmergencyOffController(_emergencyServiceMock.Object);
        }

        // Helper method to create a valid request
        private EmergencyOffRequestDTO CreateValidRequest()
        {
            return new EmergencyOffRequestDTO
            {
                OffDate = DateTime.Today.AddDays(1),
                Reason = "Health emergency"
            };
        }

        // Helper method to create a valid response
        private EmergencyOffResultDTO CreateValidResponse(Guid artistId)
        {
            return new EmergencyOffResultDTO
            {
                NailArtistId = artistId,
                OffDate = DateTime.Today.AddDays(1),
                TotalAffectedBookings = 5,
                AutoReassignedCount = 3,
                RescheduleSuggestedCount = 1,
                CancelledAndRefundedCount = 1,
                ProcessingDetails = new List<EmergencyBookingHandlingDetailDTO>
                {
                    new EmergencyBookingHandlingDetailDTO
                    {
                        BookingId = Guid.NewGuid(),
                        CustomerName = "John Doe",
                        OriginalStartTime = TimeSpan.FromHours(10),
                        HandlingResult = EmergencyHandlingResult.Reassigned,
                        NewAssignedArtistId = Guid.NewGuid(),
                        NewAssignedArtistName = "Jane Smith"
                    },
                    new EmergencyBookingHandlingDetailDTO
                    {
                        BookingId = Guid.NewGuid(),
                        CustomerName = "Alice Johnson",
                        OriginalStartTime = TimeSpan.FromHours(14),
                        HandlingResult = EmergencyHandlingResult.RescheduleSuggested,
                        SuggestedStartTime = TimeSpan.FromHours(15)
                    }
                }
            };
        }

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task SetArtistOffDuty_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            var response = CreateValidResponse(artistId);

            var apiResult = new ApiSuccessResult<EmergencyOffResultDTO>(response, "emergency off created successfully");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<EmergencyOffResultDTO>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("emergency off created successfully", value.Message);
            Assert.Equal(artistId, value.Data.NailArtistId);
            Assert.Equal(request.OffDate, value.Data.OffDate);
            Assert.Equal(5, value.Data.TotalAffectedBookings);
            Assert.Equal(3, value.Data.AutoReassignedCount);
            Assert.Equal(1, value.Data.RescheduleSuggestedCount);
            Assert.Equal(1, value.Data.CancelledAndRefundedCount);

            _emergencyServiceMock.Verify(x => x.SetArtistOffDutyAsync(artistId, request), Times.Once);
        }

        // ✅ UTCID02 - Nail artist not found → Returns 400 BadRequest
        [Fact]
        public async Task SetArtistOffDuty_UTCID02_NailArtistNotFound_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "nail artist not found");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail artist not found", value.Message);

            _emergencyServiceMock.Verify(x => x.SetArtistOffDutyAsync(artistId, request), Times.Once);
        }

        // ✅ UTCID03 - Invalid off day (past date) → Returns 400 BadRequest
        [Fact]
        public async Task SetArtistOffDuty_UTCID03_InvalidOffDay_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.OffDate = DateTime.Today.AddDays(-1); // ❌ Past date

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "invalid off day");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid off day", value.Message);

            _emergencyServiceMock.Verify(x => x.SetArtistOffDutyAsync(artistId, request), Times.Once);
        }

        // ✅ UTCID04 - Reason is required → Returns 400 BadRequest
        [Fact]
        public async Task SetArtistOffDuty_UTCID04_ReasonRequired_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.Reason = ""; // ❌ Empty reason

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "reason is required");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("reason is required", value.Message);

            _emergencyServiceMock.Verify(x => x.SetArtistOffDutyAsync(artistId, request), Times.Once);
        }

        // ✅ UTCID05 - No schedule in that day → Returns 400 BadRequest
        [Fact]
        public async Task SetArtistOffDuty_UTCID05_NoSchedule_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "nail artist does not have schedule to create off request");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail artist does not have schedule to create off request", value.Message);

            _emergencyServiceMock.Verify(x => x.SetArtistOffDutyAsync(artistId, request), Times.Once);
        }

        // ✅ Extra: Test with OffDate as today (should be allowed or not based on business rules)
        [Fact]
        public async Task SetArtistOffDuty_OffDateToday_ReturnsOk()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.OffDate = DateTime.Today; // Today's date
            var response = CreateValidResponse(artistId);

            var apiResult = new ApiSuccessResult<EmergencyOffResultDTO>(response, "emergency off created successfully");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<EmergencyOffResultDTO>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("emergency off created successfully", value.Message);
        }

        // ✅ Extra: Test with null reason
        [Fact]
        public async Task SetArtistOffDuty_NullReason_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.Reason = null!; // ❌ Null reason

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "reason is required");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("reason is required", value.Message);
        }

        // ✅ Extra: Test with OffDate = DateTime.MinValue
        [Fact]
        public async Task SetArtistOffDuty_MinDate_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.OffDate = DateTime.MinValue;

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "invalid off day");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid off day", value.Message);
        }

        // ✅ Extra: Test with off date more than 30 days in future
        [Fact]
        public async Task SetArtistOffDuty_FarFutureDate_ReturnsBadRequest()
        {
            // Arrange
            var artistId = Guid.NewGuid();
            var request = CreateValidRequest();
            request.OffDate = DateTime.Today.AddDays(60); // 60 days in future

            var apiResult = new ApiResult<EmergencyOffResultDTO>(false, "off date cannot be more than 30 days in advance");

            _emergencyServiceMock
                .Setup(x => x.SetArtistOffDutyAsync(artistId, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.SetArtistOffDuty(artistId, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<EmergencyOffResultDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("off date cannot be more than 30 days in advance", value.Message);
        }
    }
}