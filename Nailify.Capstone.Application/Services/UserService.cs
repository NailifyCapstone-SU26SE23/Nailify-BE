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
            //var validationError = ValidateUserCredentials(request.Email, request.Password, request.ConfirmPassword);
            //if (validationError != null) return validationError;

            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted) return new ApiResult<UserDto>(false, "Địa chỉ email đã tồn tại trong hệ thống.");

            var user = _mapper.Map<User>(request);
            user = FinalizeUserSetup(user, request.Password, request.Role ?? "Customer");
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
            //var validationError = ValidateUserCredentials(request.Email, request.Password, request.ConfirmPassword);
            //if (validationError != null) return validationError;

            var isEmailExisted = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email) != null;
            if (isEmailExisted) return new ApiResult<UserDto>(false, "Địa chỉ email đã được sử dụng bởi tài khoản khác.");

            var user = _mapper.Map<User>(request);
            user.AvatarUrl = "https://res.cloudinary.com/dym0se5if/image/upload/v1780664698/user1_lmcxpp.png"; // ảnh mặc định, khi nào vui thì thay

            user = FinalizeUserSetup(user, request.Password, "Customer");
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
        public async Task<ApiResult<UserDto>> GetProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
            {
                return new ApiResult<UserDto>(false, "Không tìm thấy tài khoản người dùng hoặc tài khoản đã bị vô hiệu hóa.");
            }

            // ánh xạ tự động qua AutoMapper
            var dto = _mapper.Map<UserDto>(user);

            return new ApiSuccessResult<UserDto>(dto, "Lấy thông tin hồ sơ tài khoản thành công.");
        }

        public async Task<ApiResult<UserDto>> UpdateProfileAsync(Guid userId, ProfileUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
                return new ApiResult<UserDto>(false, "Không tìm thấy tài khoản.");

            // Dùng AutoMapper ánh xạ tự động thay vì gán tay
            _mapper.Map(request, user);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Map lại sang DTO để trả về
            var dto = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(dto, "Cập nhật thông tin cá nhân thành công.");
        }

        // quản lý customer

        public async Task<ApiResult<CustomerProfileDto>> GetCustomerProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy tài khoản người dùng hoặc tài khoản đã bị vô hiệu hóa.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);

            //  hàm Mapping DTO
            var profileDto = _mapper.Map<CustomerProfileDto>(user);

            if (customer != null)
            {
                _mapper.Map(customer, profileDto);
            }

            return new ApiSuccessResult<CustomerProfileDto>(profileDto, "Lấy thông tin hồ sơ cá nhân khách hàng thành công.");
        }

        public async Task<ApiResult<CustomerProfileDto>> UpdateCustomerPreferencesAsync(Guid userId, CustomerPreferencesUpdateRequest request)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy hồ sơ khách hàng.");

            //customer.SkinTone = request.SkinTone;
            //customer.Occupation = request.Occupation;
            //customer.NailCondition = request.NailCondition;
            //customer.PersonaId = request.PersonaId;
            _mapper.Map(request, customer);

            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var updatedProfileResult = await GetCustomerProfileAsync(userId);

            return new ApiSuccessResult<CustomerProfileDto>(updatedProfileResult.Data, "Cập nhật đặc điểm sở thích cá nhân thành công.");
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

                var profileDto = _mapper.Map<CustomerProfileDto>(user);

                if (customer != null)
                {
                    _mapper.Map(customer, profileDto);
                }
                customerProfiles.Add(profileDto);
            }

            var resultPagedList = new PagedList<CustomerProfileDto>(customerProfiles, pagedUsers.MetaData.TotalItems, pageNumber, pageSize);
            return new ApiSuccessResult<PagedList<CustomerProfileDto>>(resultPagedList, "Lấy danh sách khách hàng phân trang thành công.");
        }


        /// <summary>
        /// hàm dùng chung, tránh duplicate
        /// </summary>
        /// <returns></returns>
        //private ApiResult<UserDto>? ValidateUserCredentials(string? email, string? password, string? confirmPassword)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //        return new ApiResult<UserDto>(false, "Email không được để trống.");
        //    if (password != confirmPassword)
        //        return new ApiResult<UserDto>(false, "Mật khẩu và xác nhận mật khẩu không khớp.");
        //    return null;
        //}

        private User FinalizeUserSetup(User user, string plainPassword, string role)
        {
            user.UserId = Guid.NewGuid();
            user.Password = _passwordHasher.HashPassword(plainPassword); // Băm mật khẩu
            user.Status = "Active";
            user.Role = role;
            return user;
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
                    //SkinTone = "Unknown",
                    //Occupation = "Unknown",
                    //NailCondition = "Unknown",
                    //PersonaId = Guid.NewGuid().ToString()
                };
                await _unitOfWork.CustomerRepository.CreateAsync(customer);
            }
            else if (user.Role != "Admin" && user.Role != "Manager")
            {
                return "Vai trò sẽ được cập nhật trong tương lai.";
            }

            return null; // Không có lỗi
        }

        
    }

}



