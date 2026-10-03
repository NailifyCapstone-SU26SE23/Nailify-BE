using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum WalletStatus
    {
        Active = 1,   // Đang hoạt động
        Frozen = 2,   // Tạm đóng băng
        Locked = 3    // Bị khóa
    }
}
