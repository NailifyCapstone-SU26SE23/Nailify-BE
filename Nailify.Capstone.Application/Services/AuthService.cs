using AutoMapper;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork, 
            IJwtProvider jwtProvider, 
            IPasswordHasher passwordHasher, 
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;       
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return null;

            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);
            if (!isPasswordValid) return null;

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
            };
        }

        public async Task<ApiResult<UserDto>> RegisterAsync(UserRegisterRequest request)
        {
            var isEmailExisted = await _unitOfWork.UserRepository.ExistsAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());
            if (isEmailExisted) 
            {
                return new ApiResult<UserDto>(false, "Địa chỉ email đã được sử dụng bởi tài khoản khác.");
            }

            var user = _mapper.Map<User>(request);
            user.Password = _passwordHasher.HashPassword(request.Password);
            user.Role = Nailify.Capstone.Domain.Enums.UserRole.Customer;
            user.Status = "Active";

            await _unitOfWork.UserRepository.CreateAsync(user);

            var customer = new Customer
            {
                User = user,
                LoyaltyPoint = 0
            };
            await _unitOfWork.CustomerRepository.CreateAsync(customer);

            await _unitOfWork.SaveChangesAsync();
            return new ApiSuccessResult<UserDto>(_mapper.Map<UserDto>(user), "Đăng ký tài khoản thành công.");
        }
    }
}
