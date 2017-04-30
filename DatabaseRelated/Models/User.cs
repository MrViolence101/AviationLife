using core.DatabaseRelated.Models.AirlineRelated;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace core.DatabaseRelated.Models
{
    public class User
    {

        public int playerID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public DateTime dob { get; set;}
        public float posx { get; set; }
        public float posy { get; set; }
        public float posz { get; set; }
        public int alevel { get; set; }
        public int arank { get; set; }
        public int cash { get; set; }
        public Boolean isBanned { get; set; }
        public Boolean isOnline { get; set; }
        public Airline airline { get; set; }
    }
}
