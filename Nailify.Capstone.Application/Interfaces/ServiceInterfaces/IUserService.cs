using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    /// <summary>
    /// Giao diện định nghĩa các nghiệp vụ xử lý người dùng (CRUD).
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Lấy danh sách người dùng phân trang kèm theo từ khóa tìm kiếm.
        /// </summary>
        Task<ApiResult<PagedList<UserDto>>> GetPagedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null);

        /// <summary>
        /// Lấy thông tin chi tiết một người dùng bằng ID.
        /// </summary>
        Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id);

        /// <summary>
        /// Tạo một tài khoản người dùng mới (dành cho Admin).
        /// </summary>
        Task<ApiResult<UserDto>> CreateUserAsync(UserCreateRequest request);

        /// <summary>
        /// Cập nhật thông tin tài khoản người dùng.
        /// </summary>
        Task<ApiResult<UserDto>> UpdateUserAsync(Guid id, UserUpdateRequest request);

        /// <summary>
        /// Xóa người dùng (Soft Delete - chuyển trạng thái hoạt động).
        /// </summary>
        Task<ApiResult<bool>> DeleteUserAsync(Guid id);

        /// <summary>
        /// Cho phép khách hàng đăng ký tài khoản tự động.
        /// </summary>
        Task<ApiResult<UserDto>> RegisterAsync(UserRegisterRequest request);
    }
}
