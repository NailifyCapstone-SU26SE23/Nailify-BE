using Microsoft.AspNetCore.Mvc;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailArtistBreakRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs.NailArtistBreakResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Presentation.Controllers;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class ArtistBreakCreate
    {
        private readonly Mock<INailArtistBreakService> _breakServiceMock;
        private readonly NailArtistBreaksController _controller;

        public ArtistBreakCreate()
        {
            _breakServiceMock = new Mock<INailArtistBreakService>();
            _controller = new NailArtistBreaksController(_breakServiceMock.Object);
        }

        // Helper method to create a valid request
        private NailArtistBreakCreateRequestDTO CreateValidRequest()
        {
            return new NailArtistBreakCreateRequestDTO
            {
                NailArtistId = Guid.NewGuid(),
                BreakDate = DateTime.Today.AddDays(1),
                StartTime = "14:00",
                EndTime = "15:00",
                Reason = "Personal appointment"
            };
        }

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task CreateBreak_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            var request = CreateValidRequest();
            var responseDto = new NailArtistBreakResponseDTO
            {
                NailArtistBreakId = Guid.NewGuid(),
                NailArtistId = request.NailArtistId,
                BreakDate = request.BreakDate,
                StartTime = TimeSpan.Parse(request.StartTime),
                EndTime = TimeSpan.Parse(request.EndTime),
                Reason = request.Reason,
                Status = ArtistBreakStatus.Pending.ToString()
            };

            var apiResult = new ApiSuccessResult<NailArtistBreakResponseDTO>(responseDto, "break request created successfully");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<NailArtistBreakResponseDTO>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("break request created successfully", value.Message);
            Assert.Equal(request.NailArtistId, value.Data.NailArtistId);
            Assert.Equal(request.BreakDate, value.Data.BreakDate);
            Assert.Equal(TimeSpan.Parse(request.StartTime), value.Data.StartTime);
            Assert.Equal(TimeSpan.Parse(request.EndTime), value.Data.EndTime);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ UTCID02 - Nail artist not found → Returns 400 BadRequest
        [Fact]
        public async Task CreateBreak_UTCID02_NailArtistNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "nail artist not found");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail artist not found", value.Message);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ UTCID03 - Invalid break date → Returns 400 BadRequest
        [Fact]
        public async Task CreateBreak_UTCID03_InvalidBreakDate_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.BreakDate = DateTime.Today.AddDays(-1); // ❌ Past date

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "invalid break date");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid break date", value.Message);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ UTCID04 - Invalid start time/end time → Returns 400 BadRequest
        [Fact]
        public async Task CreateBreak_UTCID04_InvalidStartEndTime_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.StartTime = "18:00"; // ❌ Invalid (end time is earlier)
            request.EndTime = "14:00";

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "invalid start time/end time");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid start time/end time", value.Message);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ UTCID05 - Reason is required → Returns 400 BadRequest
        [Fact]
        public async Task CreateBreak_UTCID05_ReasonRequired_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Reason = null; // ❌ Missing reason

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "reason is required");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("reason is required", value.Message);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ UTCID06 - No schedule in that day → Returns 400 BadRequest
        [Fact]
        public async Task CreateBreak_UTCID06_NoSchedule_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "you do not have schedule in that day to send a break request");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("you do not have schedule in that day to send a break request", value.Message);

            _breakServiceMock.Verify(x => x.CreateBreakAsync(request), Times.Once);
        }

        // ✅ Extra: Test with empty reason string
        [Fact]
        public async Task CreateBreak_EmptyReason_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.Reason = ""; // ❌ Empty reason

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "reason is required");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("reason is required", value.Message);
        }

        // ✅ Extra: Test with empty start time
        [Fact]
        public async Task CreateBreak_EmptyStartTime_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.StartTime = "";

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "start time is required");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("start time is required", value.Message);
        }

        // ✅ Extra: Test with empty end time
        [Fact]
        public async Task CreateBreak_EmptyEndTime_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.EndTime = "";

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "end time is required");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("end time is required", value.Message);
        }

        // ✅ Extra: Test with invalid time format
        [Fact]
        public async Task CreateBreak_InvalidTimeFormat_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();
            request.StartTime = "25:00"; // ❌ Invalid format

            var apiResult = new ApiResult<NailArtistBreakResponseDTO>(false, "invalid start time format");

            _breakServiceMock
                .Setup(x => x.CreateBreakAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailArtistBreakResponseDTO>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid start time format", value.Message);
        }
    }
}