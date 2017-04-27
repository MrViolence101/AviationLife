using core.DatabaseRelated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.PlayerRelated
{
    public class PlayerFunctions
    {
        private MyDbContext _context;

        public PlayerFunctions(MyDbContext context)
        {
            _context = context;
        }

        public bool doesPlayerExist(string username)
        {
            return _context.players.FirstOrDefault(x => x.username.Equals(username)) != null;
        }

    }
}
