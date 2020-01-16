using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Supplier
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public List<Product> Products { get; } = new List<Product>();
    }
}
