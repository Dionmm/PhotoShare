namespace PhotoShare.DataAccess.Entities
{
    public class Purchase : BaseEntity
    {
        public decimal Price { get; set; }
        public virtual User User { get; set; }
        public virtual Photo Photo { get; set; }
    }
}