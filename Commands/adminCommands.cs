using core.DatabaseRelated;
using core.PlayerRelated;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.Parameters;
using SampSharp.GameMode.SAMP.Commands.ParameterTypes;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using core.DatabaseRelated.Models;
using System.Threading.Tasks;
using core.Dialogs;
using core.AirlineRelated;
using Microsoft.EntityFrameworkCore;
using core.Dialogs.AirlineDialogs;
using core.Dialogs.PlayerDialogs;
using core.DatabaseRelated.Models.AirlineRelated;

namespace core.Commands
{
    class adminCommands
    {

        [Command("acmds")]
        public static void AdminCmdsCommand(Player player)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            var sb = new StringBuilder();
            sb.Append($"/asay, /setcash, /acreate, /aedit, /aleader, /usersearch, /asearch{Environment.NewLine}");
            sb.Append($"/banlist, /oban, /warn, /setadmin, /kick, /ban");

        }


        [Command("asay")]
        public static void AdminSayCommand(Player player, [Parameter(typeof(TextType))] string message)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 1, processing.adminPermissions)) return;
            if (processing.ValidateVariable(player, String.IsNullOrEmpty(message), "Invalid message provided!")) return;

            BasePlayer.SendClientMessageToAll(Color.SkyBlue, $"* {processing.GetAdminStatus(player.alevel)} {player.Name}[{player.Id}]: {Color.White}{message}");
        }
        [Command("setcash")]
        public static void SetCashCommand(Player player, Player receiver, int amount)
        {
            
            if (processing.ValidateVariable(player, player.alevel < 3, processing.adminPermissions)) return;
            if (processing.ValidateVariable(player, amount < 0, "Invalid amount entered!")) return;

            receiver.Money += amount;
        }
        [Command("aleader")]
        public static void AirlineLeaderCommand(Player player, Player receiver, int aid)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 4, processing.adminPermissions)) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (processing.ValidateVariable(player, airline == null, "This airline does not exist!")) return;

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == airline.airlineID);
            if (processing.ValidateVariable(player, airlineRanks == null, "An error occurred, please contact a system administrator")) return;
            if (processing.ValidateVariable(player, airlineRanks.Count() < 1, "An error occurred, please contact a system administrator")) return;

            if (processing.ValidateVariable(player, context.players.Include(x => x.airline).FirstOrDefault(x => x.airline.airlineID == aid && x.arank == airlineRanks.Count()) != null, "There is already a leader in this airline!")) return;

            var user = context.players.FirstOrDefault(x => x.username.Equals(receiver.Name));
            if (user == null) return;

            user.airline = airline;
            user.arank = airlineRanks.Count();

            receiver.airlineID = airline.airlineID;
            receiver.airlineRank = airlineRanks.Count();

            context.SaveChanges();

            player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()} You successfully made {Color.SkyBlue.ToString()}{receiver.Name} {Color.White.ToString()}the leader of airline, {Color.SkyBlue.ToString()}{airline.airlineName} [ID:{airline.airlineID}]");
            receiver.SendClientMessage(Color.SkyBlue, $"* Administrator {player.Name} {Color.White.ToString()}has made you the leader of {Color.SkyBlue.ToString()}{airline.airlineName} [ID:{airline.airlineID}]");
        }

        [Command("usersearch", UsageMessage = "/usersearch [part of name]")]
        public static void UserSearchCommand(Player player, string partOfName)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 3, processing.adminPermissions)) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var listOfUsers = context.players.Where(x => x.username.ToLower().Contains(partOfName.ToLower())).ToList();

            if (processing.ValidateVariable(player, listOfUsers == null, "No user exists with that partial name!")) return;
            if (processing.ValidateVariable(player, listOfUsers.Count < 1, "No user exists with that partial name!")) return;

            player.SendClientMessage("");
            player.SendClientMessage(Color.SkyBlue, $"* {listOfUsers.Count} {Color.White.ToString()}users found, matching the given partial name {Color.SkyBlue.ToString()}'{partOfName}' *");

            foreach (var item in listOfUsers)
            {
                player.SendClientMessage(Color.SkyBlue, $"[ID:{item.playerID}] {Color.White.ToString()}{item.username}");
            }
        }
        [Command("asearch", UsageMessage = "Usage: /asearch [part of name]")]
        public static void AirlineSearchCommand(Player player, string partOfName)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 3, processing.adminPermissions)) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var listOfAirlines = context.airlines.Where(x => x.airlineName.ToLower().Contains(partOfName.ToLower())).ToList();
            if (processing.ValidateVariable(player, listOfAirlines == null, "No airline exists with that partial name!")) return;
            if (processing.ValidateVariable(player, listOfAirlines.Count < 1, "No airline exists with that partial name!")) return;

            player.SendClientMessage("");
            player.SendClientMessage(Color.SkyBlue, $"* {listOfAirlines.Count} {Color.White.ToString()}airlines found, matching the given partial name {Color.SkyBlue.ToString()}'{partOfName}' *");
            foreach (var item in listOfAirlines)
            {
                player.SendClientMessage(Color.SkyBlue, $"[ID:{item.airlineID}] {Color.White.ToString()}{item.airlineName}");
            }
        }
        [Command("aedit", UsageMessage = "Usage: /aedit [airline ID]")]
        public static void AirlineEditCommand(Player player, int aid)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 5, processing.adminPermissions)) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var afunc = new AirlineFunctions(context);

            if (processing.ValidateVariable(player, !afunc.doesAirlineExistByID(aid), "This airline does not exist!")) return;

            AdminEditDialogs.aedit(player, context, aid);
        }
        [Command("acreate", UsageMessage = "Usage: /acreate [rank limit] [airline name]")]
        public static void AirlineCreateCommand(Player player, int rankLimit, [Parameter(typeof(TextType))] string airlinename)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 5, processing.adminPermissions)) return;
            if (processing.ValidateVariable(player, String.IsNullOrEmpty(airlinename), "Invalid airline name given!")) return;
            if (processing.ValidateVariable(player, rankLimit < 2 || rankLimit > 8, "Invalid rank given!")) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var afunc = new AirlineFunctions(context);

            if (processing.ValidateVariable(player, afunc.doesAirlineExistByName(airlinename), "This airline already exists!")) return;

            var airline = new Airline
            {
                airlineName = airlinename,
                amotd = "N/A"
            };

            context.airlines.Add(airline);
            context.SaveChangesAsync();

            var airlineForRank = context.airlines.FirstOrDefault(x => x.airlineName.Equals(airlinename));
            if (airlineForRank == null) return;

            for (int x = 0; x < rankLimit; x++)
            {
                var rank = new Ranklist
                {
                    rankName = "N/A",
                    rankSpot = (x+1),
                    airline = airlineForRank
                };
                context.ranks.Add(rank);
            }

            context.SaveChangesAsync();

            player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}You successfully added a new " +
                                                      $"{Color.SkyBlue.ToString()}airline {Color.White.ToString()}to the system." +
                                                      $" Please use {Color.SkyBlue.ToString()}/aedit {Color.White.ToString()}for further operations.");
            
        }
        [Command("banlist")]
        public static void BanListCommand(Player player)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 5, processing.adminPermissions)) return;

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            if (processing.ValidateVariable(player, context.bans.Count() < 1, "There are currently no banned players")) return;

            playerDialogs.BanList(player, context);
        }
        [Command("oban")]
        public static void OfflineBanPlayerCommand(Player theplayer, string uname, string reason)
        {
            if (processing.ValidateVariable(theplayer, !theplayer.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(theplayer, theplayer.alevel < 4, processing.adminPermissions)) return;
            if (processing.ValidateVariable(theplayer, String.IsNullOrEmpty(reason), "Invalid reason entered!")) return;

            foreach (var p in Player.GetAll<Player>())
            {
                if (processing.ValidateVariable(theplayer, p.Name.Equals(uname),
                    $"ERROR: {Color.White.ToString()}This player is online. Just use {Color.Red.ToString()}" +
                    $"/ban")) return;
              
            }

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var user = context.players.FirstOrDefault(x => x.username.Equals(uname));
            if (processing.ValidateVariable(theplayer, user == null, "This player does not exist!")) return;
            if (processing.ValidateVariable(theplayer, user.isBanned, "This player is already banned!")) return;

            user.isBanned = true;
            var theadmin = context.players.FirstOrDefault(x => x.username.Equals(theplayer.Name));
            if (theadmin == null) return;

            var ban = new Ban
            {
                banDate = DateTime.UtcNow,
                banReason = "Offline Ban: " + reason,
                player = user,
                admin = theadmin

            };

            context.bans.Add(ban);
            context.SaveChanges();

            theplayer.SendClientMessage(Color.Red, $"* {Color.White.ToString()}You successfully {Color.Red.ToString()}Offline-Banned {Color.White.ToString()}User {Color.Red.ToString()}'{uname}'{Color.White.ToString()}.");
            theplayer.SendClientMessage(Color.Red, $"Reason: {Color.White.ToString()}{reason}");
            
            
        }
        [Command("warn")]
        public static async void WarnPlayerCommandAsync(Player player, Player target, [Parameter(typeof(TextType))] string reason)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 1, processing.adminPermissions)) return;

            if (processing.ValidateVariable(player, !target.isLoggedIn, "That player is not even logged in!")) return;

            if (processing.ValidateVariable(player, String.IsNullOrEmpty(reason), "Invalid reason entered!")) return;
            if (target.warnings == 2)
            {
                BasePlayer.SendClientMessageToAll(Color.Red, $"* {target.Name} {Color.White.ToString()}has reached their warning limit. {Color.Red.ToString()}Reason: {Color.White.ToString()}{reason}");

                await Task.Delay(1500);
                target.Kick();
            }
            target.warnings++;

            BasePlayer.SendClientMessageToAll(Color.Red, $"* {target.Name} {Color.White.ToString()}has been warned {Color.Red.ToString()}[{target.warnings}/3]. Reason: {Color.White.ToString()}{reason}");

        }

        [Command("setadmin")]
        public static void SetAdminCommand(Player player, Player target, int alevel)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 4 && !player.IsAdmin, processing.adminPermissions)) return;

            if (processing.ValidateVariable(player, !target.isLoggedIn, "This player is not even logged in yet!")) return;
            if (processing.ValidateVariable(player, alevel < 0 || alevel > 9, "You entered an incorrect admin level")) return;
         //   if (processing.ValidateVariable(player, target.alevel >= player.alevel, "You cannot set this player's admin level")) return;

            target.alevel = alevel;

            target.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}{processing.GetAdminStatus(player.alevel)} {player.Name} has set you to {Color.SkyBlue.ToString()}level {alevel} {Color.White.ToString()}admin");
            player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}You've changed {target.Name}'s admin level to {Color.SkyBlue.ToString()}{alevel}");
            
        }

        [Command("kick")]
        public static async void KickPlayerCommandAsync(Player player, Player target, [Parameter(typeof(TextType))] string reason)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 2, processing.adminPermissions)) return;

            if (processing.ValidateVariable(player, !target.isLoggedIn, "That player is not even logged in!")) return;

            if (processing.ValidateVariable(player, String.IsNullOrEmpty(reason), "Invalid reason entered!")) return;

            BasePlayer.SendClientMessageToAll(Color.Red, $"* {target.Name} {Color.White.ToString()}has been kicked{Color.Red.ToString()} Reason: {Color.White.ToString()}{reason}");
            await Task.Delay(1500);
            target.Kick();

        }

        [Command("ban")]
        public static async void BanPlayerCommandAsync(Player player, Player target, [Parameter(typeof(TextType))] string reason)
        {
            if (processing.ValidateVariable(player, !player.isLoggedIn, "You need to be logged in!")) return;
            if (processing.ValidateVariable(player, player.alevel < 3, processing.adminPermissions)) return;

            if (processing.ValidateVariable(player, !target.isLoggedIn, "That player is not even logged in!")) return;

            if (processing.ValidateVariable(player, String.IsNullOrEmpty(reason), "Invalid reason entered!")) return;

            BasePlayer.SendClientMessageToAll(Color.Red, $"* {target.Name} {Color.White.ToString()}has been banned{Color.Red.ToString()} Reason: {Color.White.ToString()}{reason}");
            await Task.Delay(1500);

            var context = new MyDbContextFactory().Create(new DbContextFactoryOptions());
            var user = context.players.FirstOrDefault(x => x.username.Equals(target.Name));
            var theadmin = context.players.FirstOrDefault(x => x.username.Equals(player.Name));
            if (user == null || theadmin == null) return;

            var ban = new Ban
            {
                banReason = reason,
                player = user,
                banDate = DateTime.UtcNow,
                admin = theadmin
            };

            user.isBanned = true;
            context.bans.Add(ban);
            context.SaveChanges();

            target.SavePlayer();
            target.Kick();
        }

    }
}
