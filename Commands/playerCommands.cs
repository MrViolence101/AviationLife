using core.DatabaseRelated;
using core.PlayerRelated;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.Parameters;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using core.Dialogs;
using core.DatabaseRelated.Models;
using core.Dialogs.AirlineDialogs;
using core.DatabaseRelated.Models.AirlineRelated;

namespace core.Commands
{
    class PlayerCommands
    {
        [Command("kill")]
        public static void KillCommand(Player player)
        {
            player.Health = 0.0f;
        }
        [Command("apply")]
        public static void AirlineApplyCommand(Player player, int aid)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You are not even logged in!")) return;
            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());

            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            var user = context.players.FirstOrDefault(x => x.username.Equals(player.Name));
            if (user == null) return;

            var pending = new PendingAirlineApp
            {
                airline = airline,
                applyingPlayer = user,
                DateApplied = DateTime.UtcNow              
            };

            context.pendingApplications.Add(pending);
            context.SaveChanges();

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == airline.airlineID).ToList();
            if (airlineRanks == null || airlineRanks.Count < 1) return;

            var playersInAirline = context.players.Include(x => x.airline).Where(x => x.airline.airlineID == aid && x.isOnline).ToList();
            if (playersInAirline == null || playersInAirline.Count < 1) return;

            var authPlayersInAirline = playersInAirline.Where(x => airlineRanks[x.arank - 1].hasAppAuth).Count();

            player.SendClientMessage(Color.LightGreen, $"* {Color.White.ToString()}You successfully applied for airline {Color.LightGreen.ToString()}'{airline.airlineName}'{Color.White.ToString()}, there are {authPlayersInAirline} players online to accept your application.");

            foreach (var p in Player.GetAll<Player>())
            {
                if (p.airlineID == airline.airlineID)
                {
                    if (airlineRanks[p.airlineRank-1].hasAppAuth)
                    {
                        p.SendClientMessage(Color.Orange, $"* {player.Name} {Color.White.ToString()}has just submitted an application to join the airline. {Color.Orange.ToString()}/ma {Color.White.ToString()}and accept.");
                    }
                }
            }
        }
     
        [Command("ademote")]
        public static void AirlineDemoteCommand(Player player, Player receiver)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You are not even logged in!")) return;
            if (processing.ValidateVariable(player, !receiver.isLoggedIn, "The target player is not even logged in!")) return;
            if (processing.ValidateVariable(player, player.airlineID == null, "Yo do not belong to any airline!")) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == player.airlineID);
            if (airline == null) return;

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == airline.airlineID).ToList();

            if (processing.ValidateVariable(player, airlineRanks.Count() != player.airlineRank, "You do not have a sufficient rank in this airline!")) return;
            if (processing.ValidateVariable(player, airline.airlineID != receiver.airlineID, "This player does not belong to your airline!")) return;


            if (processing.ValidateVariable(player, receiver.airlineRank == 1, "You cannot demote this player!")) return;

            receiver.airlineRank--;

            var demoted = context.players.FirstOrDefault(x => x.username.Equals(receiver.Name));
            if (demoted == null) return;

            demoted.arank = receiver.airlineRank;
            context.SaveChanges();

            foreach (var p in Player.GetAll<Player>())
            {
                if (p.airlineID != null)
                {
                    if (p.airlineID == player.airlineID)
                    {
                        p.SendClientMessage(Color.LightGreen, $"* Leader {player.Name} {Color.White.ToString()}has {Color.Red.ToString()}DEMOTED {Color.White.ToString()}member {Color.LightGreen.ToString()}{receiver.Name} {Color.White.ToString()}to rank {Color.LightGreen.ToString()}{airlineRanks[receiver.airlineRank - 1].rankName}");
                    }
                }

            }

        }
        [Command("apromote")]
        public static void AirlinePromoteCommand(Player player, Player receiver)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You are not even logged in!")) return;
            if (processing.ValidateVariable(player, !receiver.isLoggedIn, "The target player is not even logged in!")) return;
            if (processing.ValidateVariable(player, player.airlineID == null, "Yo do not belong to any airline!")) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == player.airlineID);
            if (airline == null) return;

            
            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == airline.airlineID).ToList();

            if (processing.ValidateVariable(player, airlineRanks.Count() != player.airlineRank, "You do not have a sufficient rank in this airline!")) return;
            if (processing.ValidateVariable(player, airline.airlineID != receiver.airlineID, "This player does not belong to your airline!")) return;

            if (processing.ValidateVariable(player, ((receiver.airlineRank == (airlineRanks.Count-1)) || (receiver.airlineRank == airlineRanks.Count)), "You cannot promote this player!")) return;

            receiver.airlineRank++;

            var promotedUser = context.players.FirstOrDefault(x => x.username.Equals(receiver.Name));
            if (promotedUser == null) return;

            promotedUser.arank = receiver.airlineRank;
            context.SaveChanges();

            foreach (var p in Player.GetAll<Player>())
            {
                if (p.airlineID != null)
                {
                    if (p.airlineID == player.airlineID)
                    {
                        p.SendClientMessage(Color.LightGreen, $"* Leader {player.Name} {Color.White.ToString()}has {Color.LightGreen.ToString()}PROMOTED {Color.White.ToString()}member {Color.LightGreen.ToString()}{receiver.Name} {Color.White.ToString()}to rank {Color.LightGreen.ToString()}{airlineRanks[receiver.airlineRank-1].rankName}");
                    }
                }
                
            }


        }
        [Command("myairline", Shortcut = "ma")]
        public static void MyAirlineCommand(Player player)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You are not even logged in!")) return;
            if (processing.ValidateVariable(player, player.airlineID == null, "Yo do not belong to any airline!")) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == player.airlineID);
            if (airline == null) return;

            MyAirlineDialogs.MyAirline(player, context, airline.airlineID);


        }
        [Command("a")]
        public static void AirlineChatCommand(Player player, [Parameter(typeof(TextType))] string message)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You are not even logged in!")) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var user = context.players.Include(x => x.airline).FirstOrDefault(x => x.username.Equals(player.Name));
            if (user == null) return;
            if(processing.ValidateVariable(player, user.airline == null, "You do not belong to any airline")) return;

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == user.airline.airlineID).ToList();

            if (processing.ValidateVariable(player, user.airline == null, "You do not belong to any airline")) return;
            if (processing.ValidateVariable(player, String.IsNullOrEmpty(message), "Invalid message entered!")) return;

            foreach (var p in Player.GetAll<Player>())
            {
                if (p.airlineID == player.airlineID)
                {
                    foreach (var rank in airlineRanks)
                    {
                        if (player.airlineRank == rank.rankSpot)
                        {
                            p.SendClientMessage(Color.LightGreen, $"* [{user.airline.airlineName}] [{rank.rankName}] {player.Name}({player.Id}): {message}");
                        }
                    }
                }
            }

        }
        [Command("admins")]
        public static void AdminsCommand(Player player)
        {
            

            int adminCount = Player.GetAll<Player>().Where(x => x.alevel > 0).Count();
            if (processing.ValidateVariable(player, adminCount < 1, "There are no admins online!")) return;


            player.SendClientMessage(Color.LightBlue, "** Administrators Online **");
            foreach (var p in Player.GetAll<Player>().Where(x => x.alevel > 0))
            {
                player.SendClientMessage(Color.White, $"{processing.GetAdminStatus(p.alevel)} {p.Name} {Color.LightBlue.ToString()}[ID:{p.Id}]");
            }
        }
    }
}
