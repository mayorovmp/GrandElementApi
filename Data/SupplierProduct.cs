namespace GrandElementApi.Data
{
    public class SupplierProduct : BaseEntity
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }

    }
}
