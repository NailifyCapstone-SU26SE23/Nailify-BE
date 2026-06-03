using Nailify.Capstone.Application.Interfaces.ServiceInterfaces;

namespace Nailify.Capstone.Infrastructure.Service
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Gọi thư viện BCrypt ở đây
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Gọi thư viện BCrypt ở đây để đối chiếu
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
