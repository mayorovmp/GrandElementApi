using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.Data
{
    public class PartRequest : BaseEntity
    {
        public int Id { get; set; }
        public int ParentRequestId { get; set; }
        public int ChildRequestId { get; set; }
        public virtual Request ChildRequest { get; set; }
        public virtual Request ParentRequest { get; set; }
    }
}
