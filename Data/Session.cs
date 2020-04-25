using System;

namespace GrandElementApi.Data
{
    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
        public virtual User User { get; set; }
    }
}
