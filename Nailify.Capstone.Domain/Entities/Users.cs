using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nailify.Capstone.Domain.Enums;

namespace Nailify.Capstone.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid? SalonId { get; set; }
        public string? Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public UserRole Role { get; set; }
        public string Status { get; set; } = "Active";
        public virtual Salon? Salon { get; set; }
    }
}
