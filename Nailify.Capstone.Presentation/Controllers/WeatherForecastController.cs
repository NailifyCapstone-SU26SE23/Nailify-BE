using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Domain.Entities;
using Nailify.Capstone.Infrastructure.DBContext;
using Nailify.Capstone.Presentation.Middlewares;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nailify.Capstone.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly NailifyDbContext _context;

        public WeatherForecastController(NailifyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Spam người dùng mẫu trong hệ thống,
        /// không phải chương trình dự báo thời tiết của VTV đâu
        /// </summary>
        [HttpPost(Name = "GetWeatherForecast")]
        public async Task<IActionResult> SpamUsers()
        {
            var spamTargets = new List<(string Email, string FirstName, string LastName, string Role )>
            {
                ("admin1@gmail.com","Ông Chủ", "Admin", "Admin"),
                //("thichtamphuc01@meomeo.com","Trụ trì", "Admin", "Admin"),
                ("artist@gmail.com","Nghệ Sĩ", "Staff_Artist", "Staff_Artist"),
                //("dreambully@mbatu.cum", "Siêu Nghệ Nhân", "Staff_Artist", "Staff_Artist"),
                ("manager1@gmail.com", "Quản lý", "Manager", "Manager"),
                //("diddy@skibidi.com", "Quản lý", "Manager", "manager"),
                ("customer1@gmail.com", "Khách", "customer", "Customer"),
                //("sirEpstein@gmail.com", "Đại gia", "customer", "Customer"),

            };

            var notificationMessages = new List<string>();
            var usersToAdd = new List<User>();

            foreach (var target in spamTargets)
            {
                // Kiểm tra xem email này đã tồn tại trong cơ sở dữ liệu hay chưa
                var isExist = await _context.Users.AnyAsync(u => u.Email == target.Email);

                if (isExist)
                {
                    notificationMessages.Add("Thông báo: Đã tồn tại tài khoản mẫu trong DB, vui lòng vào db để xem lại.");
                }
                else
                {
                    // Khởi tạo đối tượng User mới
                    var newSpamUser = new User
                    {
                        UserId = Guid.NewGuid(),
                        Email = target.Email,
                        Password = BCrypt.Net.BCrypt.HashPassword("123456"), // Mật khẩu mặc định 
                        FirstName = target.FirstName,
                        LastName = target.LastName,
                        AvatarUrl = "https://res.cloudinary.com/dym0se5if/image/upload/v1780309663/khay123_a7bsjq.jpg",
                        Status = "Active",
                        Role = target.Role // Gán đúng vai trò theo danh sách thiết lập
                    };
                    usersToAdd.Add(newSpamUser);
                }
            }

            // Tiến hành lưu hàng loạt vào cơ sở dữ liệu nếu có dữ liệu mới hợp lệ
            if (usersToAdd.Count > 0)
            {
                await _context.Users.AddRangeAsync(usersToAdd);
                await _context.SaveChangesAsync();
                notificationMessages.Add($"Thành công: Đã khởi tạo thêm {usersToAdd.Count} tài khoản mới vào hệ thống (Mật khẩu mặc định: 123456).");
            }

            return Ok(new { result = notificationMessages });
        }
    }
}
