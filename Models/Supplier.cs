using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Supplier : SupplierShort
    {
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public bool VAT { get; set; }
        public List<Product> Products { get; set; } 
    }
}
