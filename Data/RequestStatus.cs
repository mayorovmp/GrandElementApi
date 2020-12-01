using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class RequestStatus
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int OrderBy { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
        public const int COMPLETED = 2;
        public const int NEW = 1;
    }
}
