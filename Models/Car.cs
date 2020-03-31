using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Car
    {
        public int? Id { get; set; }
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public decimal? FreightPrice { get; set; }
        public bool VAT { get; set; }
        public string Unit { get; set; }
        public CarCategory CarCategory { get; set; }
    }
}
