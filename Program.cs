using core.DatabaseRelated;
using core.DatabaseRelated.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SampSharp.Core;
using SampSharp.Core.Logging;
using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Linq;

namespace core
{
    internal class GameMode : BaseMode
    {
        private Timer _timer;
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
           
            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());

            Console.WriteLine("LOADED!");
             this.SetGameModeText("LSRP v0.1");

            _timer = new Timer(TimeSpan.FromMinutes(1), false);
            int count = processing.randomMessages().Count() - 1;
            _timer.Tick += (sender, args) =>
            {
                BasePlayer.SendClientMessageToAll(Color.SkyBlue, $"* Aviation Life: {Color.LightGray}{processing.randomMessages()[new Random().Next(0, count)]}");
            };
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