using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; }
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool Vat { get; set; }
        public virtual ICollection<SupplierProductDTO> Products { get; set; }
    }

    public class SupplierOnAddDTO
    {
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool Vat { get; set; }

        public virtual ICollection<SupplierProductDTO> Products { get; set; }
    }

    public class SupplierOnEditDTO
    {
        public int Id { get; set; }
        public string LegalEntity { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public bool Vat { get; set; }

        public virtual ICollection<SupplierProductDTO> Products { get; set; }
    }

    public class SupplierProductDTO : ProductDTO {
        public decimal? Price { get; set; }
    }
}
