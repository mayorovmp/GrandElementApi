using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public string StateNumber { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public CarCategory CarCategory { get; set; }

    }
}
