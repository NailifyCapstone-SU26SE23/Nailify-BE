using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    /// <summary>
    /// Nghiệp vụ xử lý User sử dụng IUnitOfWork.UserRepository và tự động Mapping DTO.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedList<UserDto>>> GetPagedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            System.Linq.Expressions.Expression<Func<User, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                predicate = u => u.Email.ToLower().Contains(term) ||
                                 u.FirstName.ToLower().Contains(term) ||
                                 u.LastName.ToLower().Contains(term);
            }

            var pagedResult = await _unitOfWork.UserRepository.GetPagedAsync(pageNumber, pageSize, predicate);

            var mappedItems = _mapper.Map<List<UserDto>>(pagedResult.Items);
            var resultPagedList = new PagedList<UserDto>(
                mappedItems,
                pagedResult.MetaData.TotalItems,
                pageNumber,
                pageSize
            );

            return new ApiSuccessResult<PagedList<UserDto>>(resultPagedList, "Lấy danh sách người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResult<UserDto>(false, "Không tìm thấy thông tin người dùng.");
            }

            var dto = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(dto, "Lấy thông tin người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> CreateUserAsync(UserCreateRequest request)
        {
            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted)
            {
                return new ApiResult<UserDto>(false, "Địa chỉ email đã tồn tại trong hệ thống.");
            }

            var user = _mapper.Map<User>(request);
            user.UserId = Guid.NewGuid();
            user.AvatarUrl = request.AvatarUrl ?? "default-avatar.png";
            user.Status = "Active";

            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(dto, "Tạo tài khoản người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> UpdateUserAsync(Guid id, UserUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResult<UserDto>(false, "Không tìm thấy người dùng để cập nhật.");
            }

            _mapper.Map(request, user);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(dto, "Cập nhật thông tin người dùng thành công.");
        }

        public async Task<ApiResult<bool>> DeleteUserAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResult<bool>(false, "Không tìm thấy người dùng để xóa.");
            }

            _unitOfWork.UserRepository.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true, "Xóa thông tin người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> RegisterAsync(UserRegisterRequest request)
        {
            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted)
            {
                return new ApiResult<UserDto>(false, "Địa chỉ email đã được sử dụng bởi tài khoản khác.");
            }

            var user = _mapper.Map<User>(request);
            user.UserId = Guid.NewGuid();
            user.AvatarUrl = "default-avatar.png";
            user.Status = "Active";

            await _unitOfWork.UserRepository.CreateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var dto = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(dto, "Đăng ký tài khoản thành công.");
        }

        /// <summary>
        /// nhóm chức năng quản lý thông tin cá nhân của người dùng, cho phép người dùng xem và cập nhật thông tin của chính họ.
        /// 
        public async Task<UserDto?> GetProfileAsync(Guid userId)
        {
            // tìm thông tin user  lấy từ Token
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active") return null;

            // Sử dụng AutoMapper đã cấu hình để chuyển đổi thành UserDto trả về
            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateProfileAsync(Guid userId, ProfileUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active") return false;

            // thông tin cá nhân cho phép sửa
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.AvatarUrl))
            {
                user.AvatarUrl = request.AvatarUrl;
            }

            _unitOfWork.UserRepository.Update(user);
            var result = await _unitOfWork.SaveChangesAsync();

            return result > 0;
        }
    }
    }
