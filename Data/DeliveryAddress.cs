using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class DeliveryAddress : BaseEntity
    {
        public DeliveryAddress()
        {
            Contacts = new HashSet<DeliveryContact>();
            Requests = new HashSet<Request>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<DeliveryContact> Contacts { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
