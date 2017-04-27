using System;
using System.Collections.Generic;
using System.Text;

namespace core.DatabaseRelated.Models.AirlineRelated
{
    public class AirlineAnnouncement
    {
        public int announcementID { get; set; }
        public string announcement { get; set; }
        public DateTime DateCreated { get; set; }
        
        public User createdBy { get; set; }
        public Airline airline { get; set; }
    }
}
