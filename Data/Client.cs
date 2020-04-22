using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class Client : BaseEntity
    {
        public Client()
        {
            Addresses = new HashSet<DeliveryAddress>();
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DeliveryAddress> Addresses { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
