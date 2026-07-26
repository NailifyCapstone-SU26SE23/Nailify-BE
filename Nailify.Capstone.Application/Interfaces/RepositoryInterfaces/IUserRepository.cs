using Nailify.Capstone.Domain.Entities;
using System.Threading.Tasks;

namespace Nailify.Capstone.Application.Interfaces.RepositoryInterfaces
{
    /// <summary>
    /// Giao diện repository riêng biệt cho đối tượng User.
    /// </summary>
    public interface IUserRepository : IGenericRepository<User>
    {
        // Có thể bổ sung thêm các phương thức truy vấn tùy chỉnh dành riêng cho User tại đây
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByPhoneAsync(string phone);
    }
}
