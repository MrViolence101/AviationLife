using core.DatabaseRelated;
using core.DatabaseRelated.Models;
using core.PlayerRelated;
using CryptoHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Dialogs.PlayerDialogs
{
    public class playerDialogs
    {
        public static void BanList(Player player, MyDbContext context)
        {
            var tablist = new TablistDialog("Banned Users", new string[] {"User", "Ban Date"}, "Select", "Cancel");
            var bannedusers = context.bans.Select(x => x).Include(x => x.player).Include(x => x.admin).ToList();

            foreach (var item in bannedusers)
            {
                tablist.Add(new string[] { item.player.username, item.banDate.ToString() });
            }

            tablist.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;

                BanList_banInfo(player, context, bannedusers[args.ListItem]);
               
            };

            tablist.Show(player);

         
        }

        public static void BanList_banInfo(Player player, MyDbContext context, Ban thePlayer)
        {
            var info = new MessageDialog("{FFFFFF}Ban Information - " +
                thePlayer.player.username, $"{Color.White.ToString()}Ban Date: {Color.Red.ToString()}{thePlayer.banDate}" +
                $"{Environment.NewLine}{Color.White.ToString()}By Admin: {Color.Red.ToString()}{thePlayer.admin.username}" +
                $"{Environment.NewLine}{Color.White.ToString()}Ban Reason: {thePlayer.banReason}", "Unban", "Cancel");

            info.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                try
                {
                    player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}You successfully {Color.SkyBlue.ToString()}unbanned {Color.White.ToString()}user {Color.SkyBlue.ToString()}'{thePlayer.player.username}'");
                    var user = context.players.FirstOrDefault(x => x.username.Equals(thePlayer.player.username));
                    if (user == null) return;

                    user.isBanned = false;
                    context.Remove(thePlayer);

                    
                    context.SaveChanges();
                   
                }
                catch (Exception ex)
                {
                    player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}An error has just occurred. Please contact a system administrator to assist in resolving the issue.");
                }
                


            };
            info.Show(player);
        }
        public static void register(Player player)
        {
            var input = new InputDialog("Registeration", $"Welcome {player.Name} {Environment.NewLine}{"{FFFFFF}"}Please enter your desired password to start playing:",true,"Register","Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right)
                {
                    player.Kick();
                    return;
                }

                if (String.IsNullOrEmpty(args.InputText))
                {
                    player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}The entered password is invalid!");
                    register(player);
                    return;
                }

                var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());

                var user = new User
                {

                    username = player.Name,
                    password = Crypto.HashPassword(args.InputText),
                    dob = DateTime.UtcNow,
                    posx = 1958.33f,
                    posy = 1343.12f,
                    posz = 15.36f,
                    alevel = 0,
                    arank = 0,
                    airline = null,
                    isOnline = true,
                    cash = 0
                  
                };

                context.players.Add(user);
                context.SaveChanges();

                context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
                var theUser = context.players.FirstOrDefault(x => x.username.Equals(player.Name));
                if (theUser == null) return;

                player.username = player.Name;
                player.password = args.InputText;
                player.dob = DateTime.UtcNow.ToString();
                player.warnings = 0;
                player.SetSpawnInfo(0, 1, new Vector3(1958.33, 1343.12, 15.36), 0.0f);
                player.alevel = 0;
                player.airlineID = null;
                player.airlineRank = 0;

                player.ToggleSpectating(false);
                player.Spawn();

                player.isLoggedIn = true;

              

            };
            input.Show(player);
        }
        public static void login(Player player)
        {
            var input = new InputDialog("Login", $"{"{FFFFFF}"}Welcome back, {player.Name} {Environment.NewLine}{"{FFFFFF}"}Please login with your account's password:", true, "Login", "Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right)
                {
                    player.Kick();
                    return;
                }

                if (String.IsNullOrEmpty(args.InputText))
                {
                    player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}The entered password is invalid!");
                    login(player);
                    return;
                }


                var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
                var user = context.players.Include(x => x.airline).FirstOrDefault(x => x.username.Equals(player.Name));
                if (user == null)
                {
                    player.Kick();
                    return;
                }

                if (!Crypto.VerifyHashedPassword(user.password, args.InputText))
                {
                    player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}The password you entered is incorrect and does not match, please try again!");
                    login(player);
                    return;
                }

                user.isOnline = true;

                context.SaveChangesAsync();
                player.username = user.username;
                player.password = user.password;
                if (user.airline == null)
                {
                    player.airlineID = null;
                } 
                else
                {
                    player.airlineID = user.airline.airlineID;
                }
                player.airlineRank = user.arank;
                player.Money = user.cash;
                player.dob = user.dob.ToString();
                player.alevel = user.alevel;
                player.warnings = 0;
                player.SetSpawnInfo(0, 1, new Vector3(user.posx, user.posy, user.posz), 0.0f);

                player.ToggleSpectating(false);
                player.Spawn();

                player.isLoggedIn = true;
                if (user.airline != null)
                {
                    var theAirline = context.airlines.FirstOrDefault(x => x.airlineID == user.airline.airlineID);
                    if (theAirline != null)
                    {
                        player.SendClientMessage(Color.Orange, $"AMOTD{Color.Red.ToString()}: {Color.White.ToString()}{theAirline.amotd}");
                    }
                }

               
            };
            input.Show(player);
        }
    }
}
