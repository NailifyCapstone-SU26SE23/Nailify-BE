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
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
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
            //  hàm Validate chung
            var validationError = ValidateUserCredentials(request.Email, request.Password, request.ConfirmPassword);
            if (validationError != null) return validationError;

            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted) return new ApiResult<UserDto>(false, "Địa chỉ email đã tồn tại trong hệ thống.");

            var user = InitializeBaseUser(request.Email, request.Password, request.AvatarUrl, request.Role ?? "Customer");
            await _unitOfWork.UserRepository.CreateAsync(user);

            //  hàm xử lý Role
            var roleError = await CreateRoleSpecificEntityAsync(user, request.SalonId);
            if (roleError != null) return new ApiResult<UserDto>(false, roleError);

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<UserDto>(_mapper.Map<UserDto>(user), "Tạo tài khoản người dùng thành công.");
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

        // đăng kí tk cho khách
        public async Task<ApiResult<UserDto>> RegisterAsync(UserRegisterRequest request)
        {
            // hàm Validate chung
            var validationError = ValidateUserCredentials(request.Email, request.Password, request.ConfirmPassword);
            if (validationError != null) return validationError;

            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted) return new ApiResult<UserDto>(false, "Địa chỉ email đã được sử dụng bởi tài khoản khác.");

            var user = InitializeBaseUser(request.Email, request.Password, null, "Customer");
            await _unitOfWork.UserRepository.CreateAsync(user);

            //  hàm xử lý Role
            var roleError = await CreateRoleSpecificEntityAsync(user, null);
            if (roleError != null) return new ApiResult<UserDto>(false, roleError);

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<UserDto>(_mapper.Map<UserDto>(user), "Đăng ký tài khoản thành công.");
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

        // quản lý customer

        public async Task<ApiResult<CustomerProfileDto>> GetCustomerProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy tài khoản người dùng hoặc tài khoản đã bị vô hiệu hóa.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);

            // Tái sử dụng hàm Mapping DTO
            var profileDto = MapToCustomerProfileDto(user, customer);

            return new ApiSuccessResult<CustomerProfileDto>(profileDto, "Lấy thông tin hồ sơ cá nhân khách hàng thành công.");
        }

        public async Task<ApiResult<bool>> UpdateCustomerPreferencesAsync(Guid userId, CustomerPreferencesUpdateRequest request)
        {
            // Tìm kiếm hồ sơ đặc thù trong phân hệ Customer
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResult<bool>(false, "Không tìm thấy thông tin hồ sơ khách hàng tương ứng.");
            }

            // Cập nhật các đặc điểm phục vụ động cơ gợi ý móng nghệ thuật
            customer.SkinTone = request.SkinTone;
            customer.Occupation = request.Occupation;
            customer.NailCondition = request.NailCondition;
            customer.PersonaId = request.PersonaId;

            _unitOfWork.CustomerRepository.Update(customer);
            var result = await _unitOfWork.SaveChangesAsync();

            return new ApiSuccessResult<bool>(result > 0, "Cập nhật đặc điểm sở thích cá nhân thành công.");
        }

        public async Task<ApiResult<PagedList<CustomerProfileDto>>> GetPagedCustomersAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            System.Linq.Expressions.Expression<Func<User, bool>> predicate = u => u.Role == "Customer";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                predicate = u => u.Role == "Customer" &&
                                 (u.Email.ToLower().Contains(term) ||
                                  u.FirstName.ToLower().Contains(term) ||
                                  u.LastName.ToLower().Contains(term));
            }

            var pagedUsers = await _unitOfWork.UserRepository.GetPagedAsync(pageNumber, pageSize, predicate);
            var customerProfiles = new List<CustomerProfileDto>();

            foreach (var user in pagedUsers.Items)
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(user.UserId);
                // 3. Tái sử dụng hàm Mapping DTO trong vòng lặp
                customerProfiles.Add(MapToCustomerProfileDto(user, customer));
            }

            var resultPagedList = new PagedList<CustomerProfileDto>(customerProfiles, pagedUsers.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<CustomerProfileDto>>(resultPagedList, "Lấy danh sách khách hàng phân trang thành công.");
        }


        /// <summary>
        /// hàm dùng chung, tránh duplicate
        /// </summary>
        /// <returns></returns>
        private ApiResult<UserDto>? ValidateUserCredentials(string? email, string? password, string? confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new ApiResult<UserDto>(false, "Email không được để trống.");
            if (password != confirmPassword)
                return new ApiResult<UserDto>(false, "Mật khẩu và xác nhận mật khẩu không khớp.");
            return null;
        }

        private User InitializeBaseUser(string email, string password, string? avatarUrl, string role)
        {
            return new User
            {
                UserId = Guid.NewGuid(),
                Email = email,
                Password = _passwordHasher.HashPassword(password),
                AvatarUrl = avatarUrl ?? "default-avatar.png",
                Status = "Active",
                Role = role
            };
        }

        private async Task<string?> CreateRoleSpecificEntityAsync(User user, Guid? salonId)
        {
            if (user.Role == "Staff_Artist")
            {
                if (salonId == null || salonId == Guid.Empty)
                    return "Tài khoản thợ làm móng (Staff_Artist) bắt buộc phải chỉ định cơ sở Salon làm việc.";

                var nailArtist = new NailArtist
                {
                    NailArtistId = Guid.NewGuid(),
                    AccountId = user.UserId,
                    SalonId = salonId.Value,
                    Status = "Active"
                };
                await _unitOfWork.NailArtistRepository.CreateAsync(nailArtist);
            }
            else if (user.Role == "Customer")
            {
                var customer = new Customer
                {
                    UserId = user.UserId,
                    LoyaltyPoint = 0,
                    SkinTone = "Unknown",
                    Occupation = "Unknown",
                    NailCondition = "Unknown",
                    PersonaId = Guid.NewGuid().ToString()
                };
                await _unitOfWork.CustomerRepository.CreateAsync(customer);
            }
            else if (user.Role != "Admin" && user.Role != "Manager")
            {
                return "Vai trò không tồn tại trong hệ thống.";
            }

            return null; // Không có lỗi
        }

        private CustomerProfileDto MapToCustomerProfileDto(User user, Customer? customer)
        {
            return new CustomerProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                Status = user.Status,
                Role = user.Role,
                LoyaltyPoint = customer?.LoyaltyPoint ?? 0,
                SkinTone = customer?.SkinTone ?? string.Empty,
                Occupation = customer?.Occupation ?? string.Empty,
                NailCondition = customer?.NailCondition ?? string.Empty,
                PersonaId = customer?.PersonaId ?? string.Empty
            };
        }
    }

}



