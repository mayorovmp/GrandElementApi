using System.Collections.Generic;

namespace GrandElementApi.DTOs
{
    public class DeliveryAddressDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? FreightPrice { get; set; }
        public virtual ICollection<DeliveryContactDTO> Contacts { get; set; }
    }
    public class DeliveryAddressOnAddDTO
    {
        public string Name { get; set; }
        public decimal? FreightPrice { get; set; }
        public virtual ICollection<DeliveryContactOnAddDTO> Contacts { get; set; }
    }
}
