using Microsoft.AspNetCore.Mvc;
using Moq;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Presentation.Controllers;
using Xunit;

namespace Nailify.Capstone.Test
{
    public class AuthRegisterTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthRegisterTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        // ✅ UTCID01 - Valid all fields → Returns 200 OK
        [Fact]
        public async Task Register_UTCID01_ValidAllFields_ReturnsOk()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var userDto = new UserDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone
            };

            var apiResult = new ApiSuccessResult<UserDto>(userDto, "account registered successfully");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ApiSuccessResult<UserDto>>(okResult.Value);
            Assert.True(value.IsSucceeded);
            Assert.Equal("account registered successfully", value.Message);
            Assert.Equal(request.Email, value.Data.Email);
        }

        // ✅ UTCID02 - Invalid email format → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID02_InvalidEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "invalid-email", // ❌ Invalid format
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "invalid email format");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid email format", value.Message);
        }

        // ✅ UTCID03 - Empty email → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID03_EmptyEmail_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "", // ❌ Empty
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "email is required");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("email is required", value.Message);
        }

        // ✅ UTCID04 - Email already exists → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID04_EmailExists_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "existing@email.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "email already registered");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("email already registered", value.Message);
        }

        // ✅ UTCID05 - Password length < 6 (Boundary) → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID05_ShortPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "12345", // ❌ Only 5 characters
                ConfirmPassword = "12345",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "password must has at least 6 characters");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("password must has at least 6 characters", value.Message);
        }

        // ✅ UTCID06 - Empty password → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID06_EmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "", // ❌ Empty
                ConfirmPassword = "",
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "password is required");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("password is required", value.Message);
        }

        // ✅ UTCID07 - Confirm password mismatch → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID07_PasswordMismatch_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "different123", // ❌ Doesn't match
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "confirm password does not match");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("confirm password does not match", value.Message);
        }

        // ✅ UTCID08 - Empty confirm password → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID08_EmptyConfirmPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "", // ❌ Empty
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "confirm password is required");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("confirm password is required", value.Message);
        }

        // ✅ UTCID09 - Empty FirstName/LastName → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID09_EmptyName_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "", // ❌ Empty
                LastName = "", // ❌ Empty
                Phone = "1234567890"
            };

            var apiResult = new ApiResult<UserDto>(false, "first name and last name are required");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("first name and last name are required", value.Message);
        }

        // ✅ UTCID10 - Invalid phone → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID10_InvalidPhone_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "abc123" // ❌ Invalid
            };

            var apiResult = new ApiResult<UserDto>(false, "invalid phone number");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("invalid phone number", value.Message);
        }

        // ✅ UTCID11 - Empty phone → Returns 400 BadRequest
        [Fact]
        public async Task Register_UTCID11_EmptyPhone_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRegisterRequest
            {
                Email = "valid@email.com",
                Password = "password123",
                ConfirmPassword = "password123",
                FirstName = "John",
                LastName = "Doe",
                Phone = "" // ❌ Empty
            };

            var apiResult = new ApiResult<UserDto>(false, "phone number is required");
            _authServiceMock.Setup(s => s.RegisterAsync(request)).ReturnsAsync(apiResult);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<ApiResult<UserDto>>(badResult.Value);
            Assert.False(value.IsSucceeded);
            Assert.Equal("phone number is required", value.Message);
        }
    }
}