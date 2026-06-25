using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Enums;
using Nailify.Capstone.Infrastructure.Service;
using Nailify.Capstone.Presentation.Middlewares;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    /// <summary>
    /// API Quản lý Người dùng (CRUD).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly CloudinaryService _cloudinaryService;

        public UsersController(IUserService userService, CloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Lấy danh sách người dùng phân trang (Hỗ trợ tìm kiếm theo Email/Tên/Họ, lọc theo vai trò hoặc salon).
        /// </summary>
        /// <param name="pageNumber">Số trang cần lấy (mặc định: 1).</param>
        /// <param name="pageSize">Số phần tử trên mỗi trang (mặc định: 10).</param>
        /// <param name="searchTerm">Từ khóa tìm kiếm theo tên, họ hoặc email.</param>
        /// <param name="role">Vai trò của tài khoản cần lọc (ví dụ: Manager, Receptionist, Staff_Artist, Customer, Admin).</param>
        /// <param name="salonId">ID chi nhánh Salon cần lọc.</param>
        /// <returns>Danh sách người dùng phân trang.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<PagedList<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10, 
            [FromQuery] string? searchTerm = null,
            [FromQuery] UserRole? role = null,
            [FromQuery] Guid? salonId = null)
        {
            var result = await _userService.GetPagedUsersAsync(pageNumber, pageSize, searchTerm, role, salonId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách nhân viên (Manager, Receptionist, Staff_Artist) theo chi nhánh Salon.
        /// </summary>
        /// <param name="salonId">ID chi nhánh Salon.</param>
        /// <param name="pageNumber">Số trang cần lấy (mặc định: 1).</param>
        /// <param name="pageSize">Số phần tử trên mỗi trang (mặc định: 10).</param>
        /// <param name="role">Vai trò cần lọc (Manager, Receptionist, Staff_Artist). Để trống nếu muốn lấy tất cả nhân viên.</param>
        [HttpGet("salon/{salonId}/staff")]
        [ProducesResponseType(typeof(ApiResult<PagedList<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSalonStaff(
            Guid salonId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] UserRole? role = null)
        {
            var result = await _userService.GetSalonStaffAsync(salonId, pageNumber, pageSize, role);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của người dùng bằng UserId.
        /// </summary>
        /// <param name="id">ID duy nhất của người dùng.</param>
        /// <returns>Thông tin chi tiết người dùng.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Tạo một người dùng mới (Chỉ dành cho Quản trị viên).
        /// </summary>
        /// <param name="request">Thông tin yêu cầu tạo tài khoản mới.</param>
        /// <param name="image">Ảnh đại diện của người dùng (tùy chọn).</param>
        /// <returns>Thông tin tài khoản vừa tạo.</returns>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromForm] UserCreateRequest request, IFormFile? image)
        {
            var uploadedUrl = string.Empty;
            try
            {
                if (image != null && image.Length > 0)
                {
                    uploadedUrl = await _cloudinaryService.UploadImageAsync(image);
                    request.AvatarUrl = uploadedUrl;
                }

                var result = await _userService.CreateUserAsync(request);
                if (!result.IsSucceeded)
                {
                    if (!string.IsNullOrEmpty(uploadedUrl))
                    {
                        await _cloudinaryService.DeleteImageAsync(uploadedUrl);
                    }
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(uploadedUrl))
                {
                    await _cloudinaryService.DeleteImageAsync(uploadedUrl);
                }
                return BadRequest(new ApiResult<object>(false, $"Tạo tài khoản thất bại khi tải ảnh đại diện lên: {ex.Message}"));
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản người dùng.
        /// </summary>
        /// <param name="id">ID của người dùng cần cập nhật.</param>
        /// <param name="request">Dữ liệu thông tin cập nhật mới.</param>
        /// <returns>Dữ liệu người dùng sau khi cập nhật thành công.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResult<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUserAsync(id, request);
            if (!result.IsSucceeded)
            {
                // Nếu không tìm thấy
                if (result.Message.Contains("không tìm thấy", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Xóa người dùng (Chuyển trạng thái hoạt động thành InActive).
        /// </summary>
        /// <param name="id">ID của người dùng cần xóa.</param>
        /// <returns>Kết quả xóa thành công.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Quản trị viên/Quản lý tìm kiếm và xem toàn bộ danh sách khách hàng kèm phân trang hệ thống.
        /// </summary>
        [HttpGet("customers")]
        [ProducesResponseType(typeof(ApiResult<PagedList<CustomerProfileDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomersPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            var result = await _userService.GetPagedCustomersAsync(pageNumber, pageSize, searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// Xem chi tiết thông tin khách hàng (bao gồm cả sở thích và điểm tích lũy).
        /// </summary>
        /// <param name="id">ID của khách hàng.</param>
        [HttpGet("customers/{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var result = await _userService.GetCustomerProfileByIdAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật chi tiết hồ sơ khách hàng (bao gồm cả điểm tích lũy và sở thích) dành cho Admin/Manager.
        /// </summary>
        /// <param name="id">ID của khách hàng.</param>
        /// <param name="request">Thông tin cập nhật mới.</param>
        [HttpPut("customers/{id}")]
        [ProducesResponseType(typeof(ApiResult<CustomerProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerProfileUpdateRequest request)
        {
            var result = await _userService.UpdateCustomerProfileByAdminAsync(id, request);
            if (!result.IsSucceeded)
            {
                if (result.Message.Contains("không tìm thấy", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Xóa/Vô hiệu hóa tài khoản khách hàng dành cho Admin/Manager.
        /// </summary>
        /// <param name="id">ID của khách hàng.</param>
        [HttpDelete("customers/{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result.IsSucceeded)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}
