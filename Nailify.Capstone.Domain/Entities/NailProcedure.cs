namespace Nailify.Capstone.Domain.Entities
{
    public class NailProcedure
    {
        public Guid NailProcedureId { get; set; }
        public int? NailVariantId { get; set; }
        public int? CustomerNailId { get; set; }
        public Guid ProcedureId { get; set; }
        public int StepOrder { get; set; }
        public string Status { get; set; } = "Active";
        public virtual NailVariant? NailVariant { get; set; }
        public virtual CustomerNail? CustomerNail { get; set; }
        public virtual Procedure Procedure { get; set; } = null!;
    }
}
