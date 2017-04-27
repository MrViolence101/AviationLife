using System;
using System.Collections.Generic;
using System.Text;

namespace core.DatabaseRelated.Models
{
    public class Ban
    {
        public int banID { get; set; }
        public string banReason { get; set; }
        public DateTime banDate { get; set; }

        public User player { get; set; }
        public User admin { get; set; }
    }
}
