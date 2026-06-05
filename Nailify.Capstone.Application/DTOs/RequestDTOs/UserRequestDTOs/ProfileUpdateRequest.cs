
namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class ProfileUpdateRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
