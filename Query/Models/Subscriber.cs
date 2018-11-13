using System;

namespace Query.Models
{
    public class Subscriber
    {
        public int subscriberId { get; set; }
        public string username { get; set; }
        public string key { get; set; }
        public string password { get; set; }
        public bool active { get; set; }
        public DateTime datecreated { get; set; }
    }
}
