using System;
using System.Collections.Generic;
using System.Text;

namespace core.DatabaseRelated.Models.AirlineRelated
{
    public class PendingAirlineApp
    {
        public int appID { get; set; }
        public DateTime DateApplied {get; set;}

        public Airline airline { get; set; }
        public User applyingPlayer { get; set; }
    }
}
