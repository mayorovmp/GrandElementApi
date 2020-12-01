using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class RequestStatusDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int OrderBy { get; set; }
    }
}
