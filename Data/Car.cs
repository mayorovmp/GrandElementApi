
using System.Collections.Generic;

namespace GrandElementApi.Data
{
    public class Car : BaseEntity
    {
        public Car()
        {

        }

        public int Id { get; set; }
        public string Owner { get; set; }
        public string Contacts { get; set; }
        public string Comments { get; set; }
        public int? Vat { get; set; }
        public virtual List<Request> Requests { get; set; }
        public virtual List<CarNumber> CarNumbers { get; set; }
    }
}
