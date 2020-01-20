using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Product : ProductShort
    {
        public decimal? Price { get; set; }
    }
}
