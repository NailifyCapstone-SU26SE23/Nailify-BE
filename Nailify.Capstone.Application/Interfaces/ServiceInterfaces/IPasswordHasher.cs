

namespace Nailify.Capstone.Application.Interfaces.ServiceInterfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
