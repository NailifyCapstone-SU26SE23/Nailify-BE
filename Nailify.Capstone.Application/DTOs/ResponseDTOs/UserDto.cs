using Nailify.Capstone.Application.Interfaces.MappingInterface;
using Nailify.Capstone.Domain.Entities;
using System;

namespace Nailify.Capstone.Application.DTOs.ResponseDTOs
{
    public class UserDto : IMapFrom<User>
    {
        /// <summary>
        /// ID duy nhất của người dùng.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Địa chỉ email đăng ký.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Số điện thoại liên hệ.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Tên của người dùng.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Họ của người dùng.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Đường dẫn ảnh đại diện.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// Trạng thái hoạt động (Active, InActive...).
        /// </summary>
        public string Status { get; set; }
        public string Role { get; set; }
    }
}
