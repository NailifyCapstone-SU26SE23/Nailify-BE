using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IUserService
    {
        Task<ApiResult<PagedList<UserDto>>> GetPagedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id);
        Task<ApiResult<UserDto>> CreateUserAsync(UserCreateRequest request);
        Task<ApiResult<UserDto>> UpdateUserAsync(Guid id, UserUpdateRequest request);
        Task<ApiResult<bool>> DeleteUserAsync(Guid id);
        /// <summary>
        /// tự cập nhật tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <param name="avatarUrl"></param>
        /// <returns></returns>
        Task<ApiResult<UserDto>> UpdateProfileAsync(Guid userId, ProfileUpdateRequest request, string? avatarUrl = null);
        Task<ApiResult<CustomerProfileDto>> UpdateCustomerPreferencesAsync(Guid userId, CustomerPreferencesUpdateRequest request);
        Task<ApiResult<PagedList<CustomerProfileDto>>> GetPagedCustomersAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<ApiResult<CustomerProfileDto>> GetCustomerProfileByIdAsync(Guid userId);
        Task<ApiResult<CustomerProfileDto>> UpdateCustomerProfileByAdminAsync(Guid userId, CustomerProfileUpdateRequest request);
        Task<ApiResult<CustomerProfileDto>> UpdateCustomerSelfProfileAsync(Guid userId, CustomerSelfProfileUpdateRequest request);
    }
}
