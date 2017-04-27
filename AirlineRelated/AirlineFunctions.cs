using core.DatabaseRelated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.AirlineRelated
{
    public class AirlineFunctions
    {
        MyDbContext _context;
        public AirlineFunctions(MyDbContext context)
        {
            _context = context;
        }

        public bool doesAirlineExistByName(string aname)
        {
            return _context.airlines.FirstOrDefault(x => x.airlineName.Equals(aname)) != null;
        }

        public bool doesAirlineExistByID(int id)
        {
            return _context.airlines.FirstOrDefault(x => x.airlineID == id) != null;
        }
    }
}
