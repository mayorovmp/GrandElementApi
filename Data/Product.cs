﻿using GrandElementApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class Product : BaseEntity
    {
        public Product() {
            Requests = new HashSet<Request>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
