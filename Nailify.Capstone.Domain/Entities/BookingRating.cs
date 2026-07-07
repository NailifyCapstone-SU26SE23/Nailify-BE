namespace Nailify.Capstone.Domain.Entities
{
    public class BookingRating
    {
        public Guid BookingRatingId { get; set; }
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public int OverallScore { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public int? ServiceQuality { get; set; }
        public int? Punctuality { get; set; }
        public int? Cleanliness { get; set; }
        public bool IsUpdated { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Customer Customer { get; set; } = null!;
    }
}
