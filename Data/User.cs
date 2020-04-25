using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class User : BaseEntity
    {
        public User() {
            Sessions = new HashSet<Session>();
            Requests = new HashSet<Request>();
        }
        public int Id { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}
