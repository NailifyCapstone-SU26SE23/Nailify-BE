using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.NailVariantRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Infrastructure.Service;
using Nailify.Capstone.Presentation.Controllers;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class NailVariantTest
    {
        private readonly Mock<INailVariantService> _nailVariantServiceMock;
        private readonly CloudinaryService _cloudinaryServiceMock;
        private readonly Mock<IValidator<NailVariantCreateRequest>> _createValidatorMock;
        private readonly Mock<IValidator<NailVariantUpdateRequest>> _updateValidatorMock;
        private readonly NailVariantsController _controller;

        public NailVariantTest()
        {
            _nailVariantServiceMock = new Mock<INailVariantService>();
            // ✅ Mock the configuration
            var configMock = new Mock<ICloudinaryConfiguration>();
            configMock.Setup(x => x.CloudName).Returns("test-cloud");
            configMock.Setup(x => x.ApiKey).Returns("test-key");
            configMock.Setup(x => x.ApiSecret).Returns("test-secret");

            // ✅ Create REAL CloudinaryService with mocked config
            _cloudinaryServiceMock = new CloudinaryService(configMock.Object);
            _createValidatorMock = new Mock<IValidator<NailVariantCreateRequest>>();
            _updateValidatorMock = new Mock<IValidator<NailVariantUpdateRequest>>();

            _controller = new NailVariantsController(
                _nailVariantServiceMock.Object,
                _cloudinaryServiceMock,
                _createValidatorMock.Object,
                _updateValidatorMock.Object
            );
        }

        #region Helper Methods

        private NailVariantCreateRequest CreateValidCreateRequest()
        {
            return new NailVariantCreateRequest
            {
                Name = "Classic French",
                NailShapeId = 1,
                NailSurfaceId = 1,
                NailDesignId = 1,
                ColorJson = "{\"colors\": [\"#FFFFFF\", \"#FFE4E1\"]}"
            };
        }

        private NailVariantUpdateRequest CreateValidUpdateRequest()
        {
            return new NailVariantUpdateRequest
            {
                Name = "Classic French Updated",
                NailShapeId = 2,
                NailSurfaceId = 2,
                NailDesignId = 2,
                ColorJson = "{\"colors\": [\"#FF0000\", \"#0000FF\"]}"
            };
        }

        private NailVariantDto CreateValidResponseDto(int id = 1)
        {
            return new NailVariantDto
            {
                NailVariantId = id,
                Name = "Classic French",
                NailShapeId = 1,
                NailSurfaceId = 1,
                NailDesignId = 1,
                Price = 150000,
                Duration = 60,
                ImageUrl = "https://cloudinary.com/image.jpg",
                ColorJson = "{\"colors\": [\"#FFFFFF\", \"#FFE4E1\"]}"
            };
        }

        private void SetupValidCreateValidator()
        {
            _createValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<NailVariantCreateRequest>(), default))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupValidUpdateValidator()
        {
            _updateValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<NailVariantUpdateRequest>(), default))
                .ReturnsAsync(new ValidationResult());
        }

        private void SetupInvalidValidator<T>(string errorMessage)
        {
            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Property", errorMessage)
            });

            _createValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<NailVariantCreateRequest>(), default))
                .ReturnsAsync(validationResult);
        }

        #endregion

        #region CREATE Tests

        // ✅ FIX: Setup validator to return successful validation
        [Fact]
        public async Task Create_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            _createValidatorMock
                .Setup(x => x.ValidateAsync(It.IsAny<NailVariantCreateRequest>(), default))
                .ReturnsAsync(new ValidationResult());

            var request = CreateValidCreateRequest();
            var image = new Mock<IFormFile>().Object;
            var responseDto = CreateValidResponseDto();
            var apiResult = new ApiSuccessResult<NailVariantDto>(responseDto, "nail variant created successfully");

            _nailVariantServiceMock
                .Setup(x => x.CreateNailVariantAsync(It.IsAny<NailVariantCreateRequest>(), It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<NailVariantDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
        }

        // ✅ UTCID02 - Name is required → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID02_NameRequired_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.Name = ""; // ❌ Empty name
            var image = new Mock<IFormFile>().Object;

            _createValidatorMock
                .Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Name", "name is required")
                }));

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiErrorResult<object>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Contains("name is required", value.Errors);

            _nailVariantServiceMock.Verify(x => x.CreateNailVariantAsync(It.IsAny<NailVariantCreateRequest>(), It.IsAny<string>()), Times.Never);
        }

        // ✅ UTCID03 - Name max length > 200 → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID03_NameMaxLengthExceeded_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidCreateRequest();
            request.Name = new string('A', 201); // ❌ 201 characters
            var image = new Mock<IFormFile>().Object;

            _createValidatorMock
                .Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Name", "name maximum length is 200")
                }));

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiErrorResult<object>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Contains("name maximum length is 200", value.Errors);

            _nailVariantServiceMock.Verify(x => x.CreateNailVariantAsync(It.IsAny<NailVariantCreateRequest>(), It.IsAny<string>()), Times.Never);
        }

        // ✅ UTCID04 - NailDesignId must be > 0 → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID04_NailDesignIdZero_ReturnsBadRequest()
        {
            // Arrange
            SetupValidCreateValidator();
            var request = CreateValidCreateRequest();
            request.NailDesignId = 0; // ❌ Zero ID
            var image = new Mock<IFormFile>().Object;

            var apiResult = new ApiResult<NailVariantDto>(false, "nailDesignId must be greater than 0");
            _nailVariantServiceMock
                .Setup(x => x.CreateNailVariantAsync(request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nailDesignId must be greater than 0", value.Message);
        }

        // ✅ UTCID05 - Nail design not found or inactive → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID05_NailDesignNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidCreateValidator();
            var request = CreateValidCreateRequest();
            request.NailDesignId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;

            var apiResult = new ApiResult<NailVariantDto>(false, "nail design not found or inactive");

            _nailVariantServiceMock
                .Setup(x => x.CreateNailVariantAsync(request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail design not found or inactive", value.Message);
        }

        // ✅ UTCID06 - Nail shape not found → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID06_NailShapeNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidCreateValidator();
            var request = CreateValidCreateRequest();
            request.NailShapeId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;

            var apiResult = new ApiResult<NailVariantDto>(false, "nail shape not found");

            _nailVariantServiceMock
                .Setup(x => x.CreateNailVariantAsync(request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail shape not found", value.Message);
        }

        // ✅ UTCID07 - Nail surface not found → Returns 400 BadRequest
        [Fact]
        public async Task Create_UTCID07_NailSurfaceNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidCreateValidator();
            var request = CreateValidCreateRequest();
            request.NailSurfaceId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;

            var apiResult = new ApiResult<NailVariantDto>(false, "nail surface not found");

            _nailVariantServiceMock
                .Setup(x => x.CreateNailVariantAsync(request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Create(request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail surface not found", value.Message);
        }

        #endregion

        #region UPDATE Tests

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task Update_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            SetupValidUpdateValidator();
            var id = 1;
            var request = CreateValidUpdateRequest();
            var image = new Mock<IFormFile>().Object;
            var existingDto = CreateValidResponseDto(id);
            var updatedDto = CreateValidResponseDto(id);
            updatedDto.Name = request.Name;
            updatedDto.NailShapeId = request.NailShapeId.Value;
            updatedDto.NailSurfaceId = request.NailSurfaceId.Value;
            updatedDto.NailDesignId = request.NailDesignId.Value;

            var existingResult = new ApiSuccessResult<NailVariantDto>(existingDto, "success");
            var updateResult = new ApiSuccessResult<NailVariantDto>(updatedDto, "nail variant updated successfully");

            // ✅ Mock GetById - returns existing variant
            _nailVariantServiceMock
                .Setup(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()))
                .ReturnsAsync(existingResult);

            // ✅ Mock Update - returns updated result
            _nailVariantServiceMock
                .Setup(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()))
                .ReturnsAsync(updateResult);

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<NailVariantDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("nail variant updated successfully", value.Message);
            Assert.Equal(request.Name, value.Data.Name);

            // ✅ Verifications
            _nailVariantServiceMock.Verify(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()), Times.Once);
            _nailVariantServiceMock.Verify(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()), Times.Once);
        }

        // ✅ UTCID02 - Config not found → Returns 404 NotFound
        [Fact]
        public async Task Update_UTCID02_VariantNotFound_ReturnsNotFound()
        {
            // Arrange
            SetupValidUpdateValidator();
            var id = 999;
            var request = CreateValidUpdateRequest();
            var image = new Mock<IFormFile>().Object;

            var apiResult = new ApiResult<NailVariantDto>(false, "Nail variant not found");
            _nailVariantServiceMock
    .Setup(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()))
    .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(notFoundResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("Nail variant not found", value.Message);

            _nailVariantServiceMock.Verify(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()), Times.Once);
            _nailVariantServiceMock.Verify(x => x.UpdateNailVariantAsync(It.IsAny<int>(), It.IsAny<NailVariantUpdateRequest>(), It.IsAny<string>()), Times.Never);
        }

        // ✅ UTCID03 - Nail shape not found → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID03_NailShapeNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidUpdateValidator();  // ✅ Make sure this is properly set up
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.NailShapeId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;
            var existingDto = CreateValidResponseDto(id);

            var existingResult = new ApiSuccessResult<NailVariantDto>(existingDto, "success");
            var apiResult = new ApiResult<NailVariantDto>(false, "nail shape not found");

            // ✅ GetById returns existing variant (NOT the error)
            _nailVariantServiceMock
                .Setup(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()))
                .ReturnsAsync(existingResult);  // ← Changed from apiResult to existingResult

            // ✅ Update fails with error
            _nailVariantServiceMock
                .Setup(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail shape not found", value.Message);

            // ✅ Verify goes here (after Act)
            _nailVariantServiceMock.Verify(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()), Times.Once);
            _nailVariantServiceMock.Verify(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()), Times.Once);
        }

        // ✅ UTCID04 - Nail surface not found → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID04_NailSurfaceNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidUpdateValidator();
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.NailSurfaceId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;
            var existingDto = CreateValidResponseDto(id);

            var existingResult = new ApiSuccessResult<NailVariantDto>(existingDto, "success");
            var apiResult = new ApiResult<NailVariantDto>(false, "nail surface not found");

            // ✅ FIX: GetById should return the EXISTING variant
            _nailVariantServiceMock
                .Setup(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()))
                .ReturnsAsync(existingResult);  // ← Changed from apiResult to existingResult

            // ✅ Then Update fails with error
            _nailVariantServiceMock
                .Setup(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail surface not found", value.Message);
        }

        // ✅ UTCID05 - Nail design not found → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID05_NailDesignNotFound_ReturnsBadRequest()
        {
            // Arrange
            SetupValidUpdateValidator();
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.NailDesignId = 999; // ❌ Non-existing
            var image = new Mock<IFormFile>().Object;
            var existingDto = CreateValidResponseDto(id);

            var existingResult = new ApiSuccessResult<NailVariantDto>(existingDto, "success");
            var apiResult = new ApiResult<NailVariantDto>(false, "nail design not found or inactive");

            // ✅ FIX: GetById should return the EXISTING variant
            _nailVariantServiceMock
                .Setup(x => x.GetNailVariantByIdAsync(It.IsAny<int>(), It.IsAny<Guid?>()))
                .ReturnsAsync(existingResult);  // ← Changed from apiResult to existingResult

            // ✅ Then Update fails with error
            _nailVariantServiceMock
                .Setup(x => x.UpdateNailVariantAsync(id, request, It.IsAny<string>()))
                .ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<NailVariantDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("nail design not found or inactive", value.Message);
        }

        // ✅ UTCID06 - Name is required → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID06_NameRequired_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Name = ""; // ❌ Empty name
            var image = new Mock<IFormFile>().Object;

            _updateValidatorMock
                .Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Name", "name is required")
                }));

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiErrorResult<object>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Contains("name is required", value.Errors);

            _nailVariantServiceMock.Verify(x => x.UpdateNailVariantAsync(It.IsAny<int>(), It.IsAny<NailVariantUpdateRequest>(), It.IsAny<string>()), Times.Never);
        }

        // ✅ UTCID07 - Name max length > 200 → Returns 400 BadRequest
        [Fact]
        public async Task Update_UTCID07_NameMaxLengthExceeded_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var request = CreateValidUpdateRequest();
            request.Name = new string('A', 201); // ❌ 201 characters
            var image = new Mock<IFormFile>().Object;

            _updateValidatorMock
                .Setup(x => x.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new[]
                {
                    new ValidationFailure("Name", "name maximum length is 200")
                }));

            // Act
            var result = await _controller.Update(id, request, image);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiErrorResult<object>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Contains("name maximum length is 200", value.Errors);

            _nailVariantServiceMock.Verify(x => x.UpdateNailVariantAsync(It.IsAny<int>(), It.IsAny<NailVariantUpdateRequest>(), It.IsAny<string>()), Times.Never);
        }
        #endregion
    }
}