using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Domain.Entities
{
    /// <summary>
    /// Nhật ký ghi nhận quy đổi tiền mặt sang điểm thưởng.
    /// </summary>
    public class PointConversionLog
    {
        public Guid ConversionLogId { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public Guid WalletTransactionId { get; set; }
        public int LoyaltyTransactionId { get; set; }
        /// <summary>
        ///  Số tiền mặt mà khách hàng đem ra quy đổi
        /// </summary>
        public decimal MoneyAmount { get; set; }
        /// <summary>
        /// Số điểm thưởng Loyalty mà khách hàng nhận được sau khi đổi
        /// </summary>
        public int PointsEarned { get; set; }
        /// <summary>
        /// Tỷ lệ quy đổi tiền sang điểm tại thời điểm giao dịch diễn ra.
        /// </summary>
        public decimal ConversionRate { get; set; } = 100m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual WalletTransaction WalletTransaction { get; set; } = null!;
        public virtual LoyaltyTransaction LoyaltyTransaction { get; set; } = null!;

    }
}
