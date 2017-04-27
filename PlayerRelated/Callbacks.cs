using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Events;
using core.DatabaseRelated;
using Microsoft.EntityFrameworkCore.Infrastructure;
using core.Dialogs;
using System.Linq;
using SampSharp.GameMode.SAMP;
using System.Threading;
using System.Threading.Tasks;
using core.Dialogs.PlayerDialogs;

namespace core.PlayerRelated
{
    public class Callbacks : Player
    {
        public override async void OnConnectedAsync(EventArgs e)
        {
            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var functions = new PlayerFunctions(context);

            if (functions.doesPlayerExist(Name))
            {
                
                ToggleSpectating(true);
                var user = context.players.FirstOrDefault(x => x.username.Equals(Name));
                if (user.isBanned)
                {
                    var bans = context.bans.FirstOrDefault(x => x.player.username.Equals(Name));
                    SendClientMessage(Color.Red, $"* {Color.White.ToString()}Can't you remember? You have been {Color.Red.ToString()}banned {Color.White.ToString()}on {Color.Red.ToString()}{bans.banDate} Reason: {Color.White.ToString()}{bans.banReason}");

                    await Task.Delay(500);
                    Kick();
                }
                playerDialogs.login(this);
                Color = Color.White;
            }
            else
            {
                ToggleSpectating(true);
                playerDialogs.register(this);
                Color = Color.White;
            }

            base.OnConnectedAsync(e);
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            if (isLoggedIn)
            {
                SavePlayer();
            }
            base.OnDisconnected(e);
        }
    }
}
