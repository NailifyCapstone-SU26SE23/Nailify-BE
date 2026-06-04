using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class UserCreateRequest : IMapFrom<User>
    {
        /// <summary>
        /// Địa chỉ email đăng ký.
        /// </summary>
        /// <example>thandt@gmail.com</example>
        public string Email { get; set; }

        /// <summary>
        /// Mật khẩu đăng nhập (tối thiểu 6 ký tự).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ (định dạng Việt Nam).
        /// </summary>
        /// <example>0987654321</example>
        public string? Phone { get; set; }

        /// <summary>
        /// Tên của người dùng.
        /// </summary>
        /// <example>Thanh</example>
        public string FirstName { get; set; }

        /// <summary>
        /// Họ của người dùng.
        /// </summary>
        /// <example>Doan</example>
        public string LastName { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện.
        /// </summary>
        /// <example>default-avatar.png</example>
        public string? AvatarUrl { get; set; }

        public string Role { get; set; }
        public Guid? SalonId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserCreateRequest, User>();
        }
    }
}
