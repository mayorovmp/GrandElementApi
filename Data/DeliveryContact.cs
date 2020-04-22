namespace GrandElementApi.Data
{
    public class DeliveryContact : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Communication { get; set; }
        public int? DeliveryAddressId { get; set; }

        public virtual DeliveryAddress DeliveryAddress { get; set; }
    }
}
