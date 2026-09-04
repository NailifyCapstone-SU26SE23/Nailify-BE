namespace Nailify.Capstone.Domain.Enums
{
    public enum LoyaltyTransactionType
    {
        Earned,  // Tích điểm từ Booking hoàn thành (+Points)
        Redeemed, // Sử dụng điểm để đổi quà (-Points)
        Reverted, // Thu hồi điểm khi hủy đơn hoàn điểm (-Points)
        Refund, // Hoàn điểm khi hủy đơn hoàn tiền (+Points)
        Adjusted
    }
}
