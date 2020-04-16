using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class ProductDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
    
    public class ProductOnAddDTO
    {
        public string Name { get; set; }
    }
}
