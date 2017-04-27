using core.DatabaseRelated;
using core.DatabaseRelated.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SampSharp.Core;
using SampSharp.Core.Logging;
using SampSharp.GameMode;
using System;
using System.Linq;

namespace core
{
    internal class GameMode : BaseMode
    {

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            
            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            
             Console.WriteLine("LOADED!");
             this.SetGameModeText("LSRP v0.1");
            var newairline = context.airlines.FirstOrDefault(x => x.airlineID == 2);
            for (int i = 0; i < 6; i++)
            {
                var user = new User
                {
                username = "Johannes" + i,
                password = "namakwa1012",
                posx = 15.0f,
                posy = 15.0f,
                posz = 15.0f,
                isOnline = false,
                isBanned = false,
                dob = DateTime.UtcNow,
                airline = newairline,
                alevel = 2,
                arank = i+1 
                };
                context.players.Add(user);
            }

            for (int i = 0; i < 6; i++)
            {
                var user = new User
                {
                    username = "Johannes" + i,
                    password = "namakwa1012",
                    posx = 15.0f,
                    posy = 15.0f,
                    posz = 15.0f,
                    isOnline = false,
                    isBanned = false,
                    dob = DateTime.UtcNow,
                    airline = newairline,
                    alevel = 2,
                    arank = i+1
                };
                context.players.Add(user);
            }
            context.SaveChanges();
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            new GameModeBuilder()
                .UseLogLevel(CoreLogLevel.Debug)
                .Use<GameMode>()
                .Run();
        }
    }
}