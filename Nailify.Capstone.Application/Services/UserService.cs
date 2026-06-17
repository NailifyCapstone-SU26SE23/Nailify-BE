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
        #region Account Management
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
            var response = new PagedList<UserDto>(
                mappedItems,
                pagedResult.MetaData.TotalItems,
                pageNumber,
                pageSize
            );

            return new ApiSuccessResult<PagedList<UserDto>>(response, "Lấy danh sách người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return new ApiResult<UserDto>(false, "Không tìm thấy thông tin người dùng.");
            }

            var response = _mapper.Map<UserDto>(user);
            response.SalonId = await GetSalonIdByUserContextAsync(user);
            return new ApiSuccessResult<UserDto>(response, "Lấy thông tin người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> CreateUserAsync(UserCreateRequest request)
        {
            var isEmailExisted = await _unitOfWork.UserRepository.ExistsAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());
            if (isEmailExisted) return new ApiResult<UserDto>(false, "Địa chỉ email đã tồn tại trong hệ thống.");

            var user = _mapper.Map<User>(request);
            user.Password = _passwordHasher.HashPassword(request.Password);
            user.Status = "Active";
            await _unitOfWork.UserRepository.CreateAsync(user);

            if (user.Role == "Customer")
            {
                var customer = new Customer
                {
                    User = user,
                    LoyaltyPoint = 0
                };
                await _unitOfWork.CustomerRepository.CreateAsync(customer);
            }

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

            var response = _mapper.Map<UserDto>(user);
            return new ApiSuccessResult<UserDto>(response, "Cập nhật thông tin người dùng thành công.");
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

            return new ApiSuccessResult<bool>(true, "Vô hiệu hóa tài khoản người dùng thành công.");
        }

        public async Task<ApiResult<UserDto>> UpdateProfileAsync(Guid userId, ProfileUpdateRequest request, string? avatarUrl = null)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
                return new ApiResult<UserDto>(false, "Không tìm thấy tài khoản.");

            if (request.Email != null)
            {
                user.Email = request.Email;
            }

            if (request.FirstName != null)
            {
                user.FirstName = request.FirstName;
            }

            if (request.LastName != null)
            {
                user.LastName = request.LastName;
            }

            if (request.Phone != null)
            {
                user.Phone = request.Phone;
            }

            if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                user.AvatarUrl = avatarUrl;
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<UserDto>(user);
            response.SalonId = await GetSalonIdByUserContextAsync(user);
            return new ApiSuccessResult<UserDto>(response, "Cập nhật thông tin cá nhân thành công.");
        }
        #endregion Account Management
        #region Customer Management
        public async Task<ApiResult<CustomerProfileDto>> UpdateCustomerPreferencesAsync(Guid userId, CustomerPreferencesUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Role != "Customer")
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy khách hàng.");

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy hồ sơ khách hàng.");

            _mapper.Map(request, customer);

            _unitOfWork.CustomerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync();

            var profileDto = _mapper.Map<CustomerProfileDto>(user);
            _mapper.Map(customer, profileDto);

            return new ApiSuccessResult<CustomerProfileDto>(profileDto, "Cập nhật đặc điểm sở thích cá nhân thành công.");
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
        public async Task<ApiResult<CustomerProfileDto>> GetCustomerProfileByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Role != "Customer")
            {
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy khách hàng.");
            }

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            var profileDto = _mapper.Map<CustomerProfileDto>(user);

            if (customer != null)
            {
                _mapper.Map(customer, profileDto);
            }

            return new ApiSuccessResult<CustomerProfileDto>(profileDto, "Lấy thông tin hồ sơ khách hàng thành công.");
        }

        public async Task<ApiResult<CustomerProfileDto>> UpdateCustomerProfileByAdminAsync(Guid userId, CustomerProfileUpdateRequest request)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy hồ sơ khách hàng.");
            }

            var user = (await _unitOfWork.UserRepository.GetByIdAsync(userId))!;

            // Cập nhật thông tin bảng User
            _mapper.Map(request, user);
            _unitOfWork.UserRepository.Update(user);

            // Cập nhật thông tin bảng Customer
            _mapper.Map(request, customer);
            _unitOfWork.CustomerRepository.Update(customer);

            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<CustomerProfileDto>(user);
            _mapper.Map(customer, response);

            return new ApiSuccessResult<CustomerProfileDto>(response, "Cập nhật hồ sơ khách hàng thành công.");
        }

        public async Task<ApiResult<CustomerProfileDto>> UpdateCustomerSelfProfileAsync(Guid userId, CustomerSelfProfileUpdateRequest request)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResult<CustomerProfileDto>(false, "Không tìm thấy hồ sơ khách hàng.");
            }

            var user = (await _unitOfWork.UserRepository.GetByIdAsync(userId))!;
            if (user.Status != "Active")
            {
                return new ApiResult<CustomerProfileDto>(false, "Tài khoản của bạn đã bị vô hiệu hóa.");
            }

            // Cập nhật thông tin User
            _mapper.Map(request, user);
            _unitOfWork.UserRepository.Update(user);

            // Cập nhật thông tin Customer
            _mapper.Map(request, customer);
            _unitOfWork.CustomerRepository.Update(customer);

            await _unitOfWork.SaveChangesAsync();

            var profileDto = _mapper.Map<CustomerProfileDto>(user);
            _mapper.Map(customer, profileDto);

            return new ApiSuccessResult<CustomerProfileDto>(profileDto, "Cập nhật hồ sơ cá nhân thành công.");
        }
        #endregion Customer Management

        private async Task<Guid?> GetSalonIdByUserContextAsync(User user)
        {
            if (user.Role == "Manager")
            {
                var salons = await _unitOfWork.SalonRepository.GetPagedAsync(1, 1, x => x.ManagerId == user.UserId);
                var salon = salons.Items.FirstOrDefault();

                return salon?.SalonId;
            }

            if (user.Role == "Staff_Artist")
            {
                var artists = await _unitOfWork.NailArtistRepository.GetPagedAsync(1, 1, x => x.AccountId == user.UserId);
                var artist = artists.Items.FirstOrDefault();

                return artist?.SalonId;
            }

            return null;
        }
    }
}



