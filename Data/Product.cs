using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
