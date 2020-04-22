using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DeliveryAddressDTO> Addresses { get; set; }
    }
    public class ClientOnAddDTO
    {
        public string Name { get; set; }
        public virtual ICollection<DeliveryAddressOnAddDTO> Addresses { get; set; }
    }
    public class ClientOnEditDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DeliveryAddressOnAddDTO> Addresses { get; set; }
    }
}
