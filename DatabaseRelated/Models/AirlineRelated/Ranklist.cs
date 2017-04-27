using System;
using System.Collections.Generic;
using System.Text;

namespace core.DatabaseRelated.Models.AirlineRelated
{
    public class Ranklist
    {
        public int rankID { get; set; }
        public string rankName { get; set; }
        public int rankSpot { get; set; }

        public Airline airline { get; set; }
    }
}
