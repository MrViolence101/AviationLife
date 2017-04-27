using core.DatabaseRelated;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.PlayerRelated
{
    [PooledType]
    public class Player : BasePlayer
    {
        public string username { get; set; }
        public string password { get; set; }
        public string dob { get; set; }
        public bool isLoggedIn { get; set; }
        public int alevel { get; set; }
        public int warnings { get; set; }
        public int? airlineID { get; set; }
        public int airlineRank { get; set; }
        public bool isOnline {get; set;}
        public void SavePlayer()
        {
            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var user = context.players.FirstOrDefault(x => x.username.Equals(Name));

            if (user == null) return;

            var vec = Position;

            user.username = username;
            user.dob = DateTime.UtcNow;
            user.posx = vec.X;
            user.posy = vec.Y;
            user.posz = vec.Z;
            user.alevel = alevel;
            user.isOnline = false;
            user.cash = Money;

            context.SaveChanges();
        }
    }
}
