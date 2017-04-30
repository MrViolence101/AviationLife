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
                    playerDialogs.YouAreBannedAsync(this, context);
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
