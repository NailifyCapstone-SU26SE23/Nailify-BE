namespace Nailify.Capstone.Domain.Entities
{
    public class BookingItem
    {
        public Guid BookingItemId { get; set; }
        public Guid BookingId { get; set; }
        public Guid? ServiceId { get; set; }
        public int? NailVariantId { get; set; }
        public int? ShapeMethodConfigId { get; set; } 
        public Guid? CustomerNailRequestId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public int Duration { get; set; }
        public virtual Booking Booking { get; set; } = null!;
        public virtual Services? Service { get; set; }
        public virtual NailVariant? NailVariant { get; set; }
        public virtual CustomerNailRequest? CustomerNailRequest { get; set; }
        public virtual ShapeMethodConfig? ShapeMethodConfig { get; set; } 
        public virtual ICollection<BookingProcedure> BookingProcedures { get; set; } = new List<BookingProcedure>();
    }
}
