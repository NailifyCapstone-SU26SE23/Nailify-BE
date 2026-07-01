using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum QueueStatus
    {
        Waiting,      // đang ngồi chờ
        Called,       // lễ tân gọi số
        InService,    // đang được phục vụ
        Done,         // xong
        Left,         // khách bỏ về (no-show)
    }
}
