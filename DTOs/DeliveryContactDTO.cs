using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class DeliveryContactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Communication { get; set; }
    }
    public class DeliveryContactOnAddDTO
    {
        public string Name { get; set; }
        public string Communication { get; set; }
    }
}
