using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class Supplier : BaseEntity
    {
        public Supplier()
        {
            Products = new HashSet<SupplierProduct>();
            Requests = new HashSet<Request>();
        }
        public int Id { get; set; }
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public int? Vat { get; set; }

        public virtual ICollection<SupplierProduct> Products{ get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
