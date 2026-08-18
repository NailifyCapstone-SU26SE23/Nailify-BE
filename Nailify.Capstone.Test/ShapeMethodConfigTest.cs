using Microsoft.AspNetCore.Mvc;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.ShapeMethodConfigRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Presentation.Controllers;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class ShapeMethodConfigTest
    {
        private readonly Mock<IShapeMethodConfigService> _serviceMock;
        private readonly ShapeMethodConfigsController _controller;

        public ShapeMethodConfigTest()
        {
            _serviceMock = new Mock<IShapeMethodConfigService>();
            _controller = new ShapeMethodConfigsController(_serviceMock.Object);
        }

        #region Helper Methods

        private ShapeMethodConfigCreateRequest CreateValidCreateRequest()
        {
            return new ShapeMethodConfigCreateRequest
            {
                NailShapeId = 1,
                Name = "Classic Round",
                Price = 150000,
                Duration = 60,
            };
        }

        private ShapeMethodConfigUpdateRequest CreateValidUpdateRequest()
        {
            return new ShapeMethodConfigUpdateRequest
            {
                NailShapeId = 1,
                Name = "Classic Round Updated",
                Price = 180000,
                Duration = 75,
            };
        }

        private ShapeMethodConfigDto CreateValidResponseDto(int id = 1)
        {
            return new ShapeMethodConfigDto
            {
                ShapeMethodConfigId = id,
                NailShapeId = 1,
                Name = "Classic Round",
                Price = 150000,
                Duration = 60,
                Status = "Active"
            };
        }

        #endregion

        #region CREATE Tests

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task Create_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var responseDto = CreateValidResponseDto();
            var apiResult = new ApiSuccessResult<ShapeMethodConfigDto>(responseDto, "shape method config created successfully");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<ShapeMethodConfigDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("shape method config created successfully", value.Message);
            Assert.Equal(request.NailShapeId, value.Data.NailShapeId);
            Assert.Equal(request.Name, value.Data.Name);
            Assert.Equal(request.Price, value.Data.Price);
            Assert.Equal(request.Duration, value.Data.Duration);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        // ✅ UTCID02 - Nail shape not found → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID02_NailShapeNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "nail shape not found");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail shape not found", value.Message);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        // ✅ UTCID03 - Name is required → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID03_NameRequired_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.Name = ""; // ❌ Empty name

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "name is required");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("name is required", value.Message);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        // ✅ UTCID04 - Name max length > 200 → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID04_NameMaxLengthExceeded_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.Name = new string('A', 201); // ❌ 201 characters

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "name maximum length is 200");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("name maximum length is 200", value.Message);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        // ✅ UTCID05 - Price cannot be negative → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID05_PriceNegative_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.Price = -10000; // ❌ Negative price

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "price cannot be negative");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("price cannot be negative", value.Message);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        // ✅ UTCID06 - Unauthorized → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID06_Unauthorized_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "you are unauthorized for this feature");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("you are unauthorized for this feature", value.Message);

            _serviceMock.Verify(x => x.CreateShapeMethodConfigAsync(request), Times.Once);
        }

        #endregion

        #region UPDATE Tests

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task Update_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            var responseDto = CreateValidResponseDto(id);
            responseDto.Name = request.Name;
            responseDto.Price = request.Price;
            responseDto.Duration = request.Duration;

            var apiResult = new ApiSuccessResult<ShapeMethodConfigDto>(responseDto, "shape method config updated successfully");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<ShapeMethodConfigDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("shape method config updated successfully", value.Message);
            Assert.Equal(id, value.Data.ShapeMethodConfigId);
            Assert.Equal(request.NailShapeId, value.Data.NailShapeId);
            Assert.Equal(request.Name, value.Data.Name);
            Assert.Equal(request.Price, value.Data.Price);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        // ✅ UTCID02 - Shape method config not found → Returns 404 NotFound
        [Fact]
        public async Task Update_UTCID02_ConfigNotFound_ReturnsNotFound()
        {
            // Arrange
            var id = 999;
            var request = CreateValidUpdateRequest();
            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "Khong tim thay cau hinh");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(notFoundResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("Khong tim thay cau hinh", value.Message);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        // ✅ UTCID03 - Nail shape not found → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID03_NailShapeNotFound_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "nail shape not found");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail shape not found", value.Message);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        // ✅ UTCID04 - Name is required → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID04_NameRequired_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Name = ""; // ❌ Empty name

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "name is required");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("name is required", value.Message);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        // ✅ UTCID05 - Name max length > 200 → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID05_NameMaxLengthExceeded_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Name = new string('A', 201); // ❌ 201 characters

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "name maximum length is 200");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("name maximum length is 200", value.Message);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        // ✅ UTCID06 - Price cannot be negative → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID06_PriceNegative_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Price = -10000; // ❌ Negative price

            var apiResult = new ApiResult<ShapeMethodConfigDto>(false, "price cannot be negative");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<ShapeMethodConfigDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("price cannot be negative", value.Message);

            _serviceMock.Verify(x => x.UpdateShapeMethodConfigAsync(id, request), Times.Once);
        }

        #endregion

        #region Extra Tests

        // ✅ Extra: Create with different nail shape ID
        [Fact]
        public async Task Create_WithDifferentNailShapeId_ReturnsOk()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.NailShapeId = 5;
            var responseDto = CreateValidResponseDto();
            responseDto.NailShapeId = 5;

            var apiResult = new ApiSuccessResult<ShapeMethodConfigDto>(responseDto, "shape method config created successfully");

            _serviceMock
                .Setup(x => x.CreateShapeMethodConfigAsync(request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<ShapeMethodConfigDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal(5, value.Data.NailShapeId);
        }

        // ✅ Extra: Update with different duration
        [Fact]
        public async Task Update_WithDifferentDuration_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Duration = 120;
            var responseDto = CreateValidResponseDto(id);
            responseDto.Duration = 120;

            var apiResult = new ApiSuccessResult<ShapeMethodConfigDto>(responseDto, "shape method config updated successfully");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<ShapeMethodConfigDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal(120, value.Data.Duration);
        }

        // ✅ Extra: Update with price = 0 (valid)
        [Fact]
        public async Task Update_WithZeroPrice_ReturnsOk()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Price = 0;
            var responseDto = CreateValidResponseDto(id);
            responseDto.Price = 0;

            var apiResult = new ApiSuccessResult<ShapeMethodConfigDto>(responseDto, "shape method config updated successfully");

            _serviceMock
                .Setup(x => x.UpdateShapeMethodConfigAsync(id, request))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<ShapeMethodConfigDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal(0, value.Data.Price);
        }

        #endregion
    }
}