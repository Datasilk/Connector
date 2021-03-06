﻿using System;
namespace Query.Models
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool photo { get; set; }
        public int status { get; set; }
        public DateTime datecreated { get; set; }
        public DateTime lastlogin { get; set; }
    }
}
