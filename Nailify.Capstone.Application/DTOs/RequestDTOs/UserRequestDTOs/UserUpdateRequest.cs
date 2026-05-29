using AutoMapper;
using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;

namespace Nailify.Capstone.Application.DTOs.RequestDTOs.UserRequestDTOs
{
    public class UserUpdateRequest : IMapFrom<User>
    {
        /// <summary>
        /// Số điện thoại liên hệ (định dạng Việt Nam).
        /// </summary>
        /// <example>0987654321</example>
        public string? Phone { get; set; }

        /// <summary>
        /// Tên của người dùng.
        /// </summary>
        /// <example>Bình</example>
        public string FirstName { get; set; }

        /// <summary>
        /// Họ của người dùng.
        /// </summary>
        /// <example>Trần</example>
        public string LastName { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện.
        /// </summary>
        /// <example>new-avatar.png</example>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (Active, InActive...).
        /// </summary>
        /// <example>Active</example>
        public string Status { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserUpdateRequest, User>();
        }
    }
}
