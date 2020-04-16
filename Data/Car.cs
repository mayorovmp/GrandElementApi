
namespace GrandElementApi.Data
{
    public class Car : BaseEntity
    {
        public Car()
        {

        }

        public int Id { get; set; }
        public string Owner { get; set; }
        public string StateNumber { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public int? CarCategoryId { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public int? Vat { get; set; }

        public virtual CarCategory CarCategory { get; set; }
    }
}
