using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Enums
{
    public enum WalletReferenceType
    {
        Booking = 1, // Liên quan đến booking
        Withdrawal = 2, // Liên quan đến rút tiền
        PayOs = 3, // Liên quan đến PayOs
        PointsConversion = 4 // Liên quan đến chuyển đổi điểm
    }
}
