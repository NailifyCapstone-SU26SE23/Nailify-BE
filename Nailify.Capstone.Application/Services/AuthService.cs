using Nailify.Capstone.Application.DTOs.RequestDTOs.AuthRequestDTOs;
using Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs;
using Nailify.Capstone.Application.DTOs.ResponseDTOs;
using Nailify.Capstone.Application.Interfaces.RepositoryInterfaces;
using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher _passwordHasher;


        public AuthService(IUnitOfWork unitOfWork, IJwtProvider jwtProvider, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _jwtProvider = jwtProvider;       
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            //Tìm tài khoản
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return null;

            // 2. Đối chiếu mật khẩu băm thông qua BCrypt
            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Password);
            if (!isPasswordValid) return null;

            // 3. Cấp Token
            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponse
            {
                Token = token,
                //Role = user.Role
            };
        }
    }
}
