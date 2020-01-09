using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Address> Addresses { get; set; }
    }
    public class Address {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<Contact> Contacts { get; set; }
    }
    public class Contact {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Communication { get; set; }
    }
}
