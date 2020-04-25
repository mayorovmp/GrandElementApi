﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandElementApi.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
    }
    public class UserLoginDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
