using System;
using System.Collections.Generic;
using System.Text;

namespace AttentionApp.Models
{
    public class Settings
    {
        public Contact LastContact { get; set; }
        public List<Message> Messages { get; set; }

        public DateTime RestrictionTime { get; set; }
        public bool Restricted { get; set; }
        public int counter { get; set; }
        public bool Locked = true;
    }
}
