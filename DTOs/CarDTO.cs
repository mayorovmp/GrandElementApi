using GrandElementApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class CarDTO
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public bool? Vat { get; set; }

        public virtual CarCategoryDTO CarCategory { get; set; }
    }

    public class CarOnAddDTO
    {
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public decimal? FreightPrice { get; set; }
        public string Unit { get; set; }
        public bool? Vat { get; set; }

        public virtual CarCategoryDTO CarCategory { get; set; }
    }
}
