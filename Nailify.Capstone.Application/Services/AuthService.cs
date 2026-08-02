using AutoMapper;
using Google.Apis.Auth;
using Microsoft.Extensions.Caching.Distributed;
using Nailify.Capstone.Application.Common;
using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.MailRequestDTO;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.ConfigurationInterfaces;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Domain.Enums;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IDistributedCache _cache;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IGoogleConfiguration _googleConfiguration;
        private readonly IMapper _mapper;
        private const string ResetPasswordCachePrefix = "forgot-password";
        private const string ResetPasswordCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int ResetPasswordCodeLength = 6;
        private static readonly TimeSpan ResetPasswordTokenTtl = TimeSpan.FromMinutes(15);

        public AuthService(
            IUnitOfWork unitOfWork, 
            IJwtProvider jwtProvider, 
            IPasswordHasher passwordHasher, 
            IDistributedCache cache,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            IGoogleConfiguration googleConfiguration,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;       
            _passwordHasher = passwordHasher;
            _cache = cache;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _googleConfiguration = googleConfiguration;
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

        public async Task<AuthResponse?> GoogleLoginAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(_googleConfiguration.ClientId))
            {
                throw new InvalidOperationException("Google ClientId is not configured.");
            }

            var payload = await GoogleJsonWebSignature.ValidateAsync(
                request.IdToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleConfiguration.ClientId }
                });

            var email = payload.Email?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                var (firstName, lastName) = SplitGoogleName(payload.Name, payload.GivenName, payload.FamilyName);
                user = new User
                {
                    Email = email,
                    Password = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N")),
                    FirstName = firstName,
                    LastName = lastName,
                    AvatarUrl = payload.Picture,
                    Role = UserRole.Customer,
                    Status = "Active"
                };

                await _unitOfWork.UserRepository.CreateAsync(user);
                await _unitOfWork.CustomerRepository.CreateAsync(new Customer
                {
                    User = user,
                    LoyaltyPoint = 0
                });
                await _unitOfWork.SaveChangesAsync();
            }

            var token = _jwtProvider.GenerateToken(user);
            return new AuthResponse
            {
                Token = token
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

        public async Task<ApiResult<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            const string responseMessage = "Nếu email tồn tại, mã đặt lại mật khẩu đã được gửi.";
            var normalizedEmail = request.Email.Trim().ToLower();
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(normalizedEmail);

            if (user == null || user.Status != "Active")
            {
                return new ApiSuccessResult<bool>(true, responseMessage);
            }

            var token = GenerateResetToken();
            var tokenHash = HashToken(token);
            var cacheKey = GetResetPasswordCacheKey(tokenHash);

            await _cache.SetStringAsync(
                cacheKey,
                user.UserId.ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ResetPasswordTokenTtl
                });

            await _emailService.SendEmailAsync(new MailRequest
            {
                ToAddress = user.Email,
                Subject = "Nailify - mã đặt lại mật khẩu",
                Body = _emailTemplateService.GenerateForgotPasswordEmail($"{user.FirstName} {user.LastName}".Trim(), token)
            });

            return new ApiSuccessResult<bool>(true, responseMessage);
        }

        public async Task<ApiResult<bool>> CheckResetPasswordTokenAsync(CheckResetPasswordTokenRequest request)
        {
            var tokenHash = HashToken(NormalizeResetCode(request.Token));
            var cacheKey = GetResetPasswordCacheKey(tokenHash);
            var userIdValue = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                return new ApiResult<bool>(false, "Mã đặt lại không hợp lệ hoặc đã hết hạn.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
            {
                await _cache.RemoveAsync(cacheKey);
                return new ApiResult<bool>(false, "Mã đặt lại không hợp lệ hoặc đã hết hạn.");
            }

            return new ApiSuccessResult<bool>(true, "Mã đặt lại hợp lệ.");
        }

        public async Task<ApiResult<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return new ApiResult<bool>(false, "Xác nhận mật khẩu không khớp.");
            }

            var tokenHash = HashToken(NormalizeResetCode(request.Token));
            var cacheKey = GetResetPasswordCacheKey(tokenHash);
            var userIdValue = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                return new ApiResult<bool>(false, "Mã đặt lại không hợp lệ hoặc đã hết hạn.");
            }

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null || user.Status != "Active")
            {
                await _cache.RemoveAsync(cacheKey);
                return new ApiResult<bool>(false, "Mã đặt lại không hợp lệ hoặc đã hết hạn.");
            }

            user.Password = _passwordHasher.HashPassword(request.NewPassword);

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            await _cache.RemoveAsync(cacheKey);

            return new ApiSuccessResult<bool>(true, "Đặt lại mật khẩu thành công.");
        }

        private static string GenerateResetToken()
        {
            Span<char> code = stackalloc char[ResetPasswordCodeLength];
            for (var i = 0; i < code.Length; i++)
            {
                var index = RandomNumberGenerator.GetInt32(ResetPasswordCodeAlphabet.Length);
                code[i] = ResetPasswordCodeAlphabet[index];
            }

            return new string(code);
        }

        private static string NormalizeResetCode(string token)
        {
            return token.Trim().Replace(" ", string.Empty).ToUpperInvariant();
        }

        private static string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes).ToLower();
        }

        private static string GetResetPasswordCacheKey(string tokenHash)
        {
            return $"{ResetPasswordCachePrefix}:{tokenHash}";
        }

        private static (string FirstName, string LastName) SplitGoogleName(string? name, string? givenName, string? familyName)
        {
            var firstName = !string.IsNullOrWhiteSpace(givenName) ? givenName.Trim() : string.Empty;
            var lastName = !string.IsNullOrWhiteSpace(familyName) ? familyName.Trim() : string.Empty;

            if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
            {
                return (string.IsNullOrWhiteSpace(firstName) ? "Google" : firstName, lastName);
            }

            var parts = (name ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return ("Google", "User");
            }

            if (parts.Length == 1)
            {
                return (parts[0], string.Empty);
            }

            return (parts[0], string.Join(' ', parts.Skip(1)));
        }
    }
}
