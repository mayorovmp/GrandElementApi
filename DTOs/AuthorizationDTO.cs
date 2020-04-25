using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class AuthorizationDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public Guid Token { get; set; }
    }
}
