using core.AirlineRelated;
using core.DatabaseRelated.Models;
using core.PlayerRelated;
using Microsoft.EntityFrameworkCore;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core.Dialogs.AirlineDialogs
{
    public class AdminEditDialogs
    {
        public static void aedit(Player player, MyDbContext context, int aid)
        {
            var theairline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (theairline == null) return;
            var list = new ListDialog($"{Color.Red.ToString()}Admin: {Color.White.ToString()}Airline Control Panel - {Color.LightGreen.ToString()}{theairline.airlineName}", "Edit", "Cancel");

            list.AddItems(new string[] { "Change Name", "Change MOTD", "Edit Ranks", "View all Members", $"{Color.Red.ToString()}Delete" });

            list.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;

                switch (args.ListItem)
                {
                    case 0:
                        aedit_ChangeName(player, context, aid);
                        break;
                    case 1:
                        aedit_ChangeMotd(player, context, aid);
                        break;
                    case 2:
                        aedit_EditRanks(player, context, aid);
                        break;
                    case 3:
                        aedit_ViewAllMembers(player, context, aid);
                        break;
                }
            };

            list.Show(player);
        }

        public static void aedit_ViewAllMembers(Player player, MyDbContext context, int aid)
        {
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            var tablist = new TablistDialog($"View All Members -  {Color.LightGreen.ToString()}{airline.airlineName}", new string[] { "User", "Status" }, "Remove", "Cancel");

            var users = context.players.Include(x => x.airline).Where(x => x.airline.airlineID == airline.airlineID).ToList();
            if (users == null)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}There are no Members for this airline");
                aedit(player, context, aid);
                return;
            }

            if (users.Count < 1)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}There are no Members for this airline");
                aedit(player, context, aid);
                return;
            }

            foreach (var item in users)
            {
                if (item.isOnline)
                {
                    tablist.Add(new string[] { $"{Color.White.ToString()}{item.username}", $"{Color.LightGreen.ToString()}Online" });
                }
                else
                {
                    tablist.Add(new string[] { $"{Color.White.ToString()}{item.username}", $"{Color.Red.ToString()}Offline" });
                }

            }

            tablist.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                if (users[args.ListItem] == null) return;

                users[args.ListItem].airline = null;
                users[args.ListItem].arank = 0;

                foreach (var p in Player.GetAll<Player>())
                {
                    if (p.Name.Equals(users[args.ListItem].username))
                    {
                        p.airlineID = null;
                        p.airlineRank = 0;
                    }
                }

                context.SaveChangesAsync();

                player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}You successfully removed user {Color.SkyBlue.ToString()}" +
                                                        $"'{users[args.ListItem].username}' {Color.White.ToString()}from {Color.SkyBlue.ToString()}" +
                                                        $"Airline ID:{aid}");
                aedit(player, context, aid);
            };
            tablist.Show(player);
        }
        public static void aedit_EditRanks(Player player, MyDbContext context, int aid)
        {
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            var tablist = new TablistDialog($"Edit Ranks - {Color.LightGreen.ToString()}{airline.airlineName}", new string[] { "Spot #", "Rank Name" }, "Edit", "Cancel");

            var airlineRanks = context.ranks.Where(x => x.airline.airlineID == aid).Include(x => x.airline).ToList();

            if (airlineRanks == null)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}This airline has no ranks!");
                aedit(player, context, aid);
                return;
            }

            if (airlineRanks.Count < 1)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}This airline has no ranks!");
                aedit(player, context, aid);
                return;
            }

            foreach (var item in airlineRanks)
            {
                tablist.Add(new string[] { item.rankSpot.ToString(), item.rankName });
            }

            tablist.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                if (processing.ValidateVariable(player, airlineRanks[args.ListItem] == null, "This rank does not exist anymore!")) return;

                aedit_EditRanks_SpecificRank(player, context, aid, airlineRanks[args.ListItem].rankSpot);

            };
            tablist.Show(player);
        }

        public static void aedit_EditRanks_SpecificRank(Player player, MyDbContext context, int aid, int rankSpot)
        {
            var specificRank = context.ranks.Include(x => x.airline).FirstOrDefault(x => x.airline.airlineID == aid && x.rankSpot == rankSpot);

            var input = new InputDialog($"{Color.White.ToString()}Spot {Color.LightGreen.ToString()}#{rankSpot}{Color.White.ToString()}:" +
                $" Edit Rank", $"{Color.White.ToString()}Current rank name on Spot #{rankSpot}: {Color.LightGreen.ToString()}{specificRank.rankName}{Environment.NewLine}" +
                $"{Color.White.ToString()}Please enter the new desired rank name:",
                false, "Edit", "Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;

                if (processing.ValidateVariable(player, String.IsNullOrEmpty(args.InputText), "Invalid rank entered!")) return;
                specificRank.rankName = args.InputText;

                context.SaveChanges();
                player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}Successfully changed rank's name in {Color.SkyBlue.ToString()}Spot #{specificRank.rankSpot} {Color.White.ToString()} to {Color.SkyBlue.ToString()}'{args.InputText}'");
                aedit_EditRanks(player, context, aid);
            };

            input.Show(player);

        }
        public static void aedit_ChangeName(Player player, MyDbContext context, int aid)
        {
            var theairline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (theairline == null) return;
            var input = new InputDialog("Change Airline Name", $"{Color.White.ToString()}Current Airline Name: " +
                                                               $"{Color.LightGreen.ToString()}{theairline.airlineName}{Environment.NewLine}" +
                                                               $"{Color.White.ToString()}Please enter the new desired airline name:", false, "Confirm", "Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;

                var afunc = new AirlineFunctions(context);
                if (processing.ValidateVariable(player, afunc.doesAirlineExistByName(args.InputText), "This airline already exist!")) return;

                theairline.airlineName = args.InputText;
                context.SaveChangesAsync();

                player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}Airline ID:{aid} name has successfully been changed to {Color.SkyBlue.ToString()}'{args.InputText}'");
                aedit(player, context, aid);
            };
            input.Show(player);
        }
        public static void aedit_ChangeMotd(Player player, MyDbContext context, int aid)
        {
            var theairline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (theairline == null) return;
            var input = new InputDialog("Change Airline MOTD", $"{Color.White.ToString()}Current Airline MOTD: " +
                                                               $"{Color.LightGreen.ToString()}{theairline.amotd}{Environment.NewLine}" +
                                                               $"{Color.White.ToString()}Please enter the new desired airline motd:", false, "Confirm", "Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                if (processing.ValidateVariable(player, String.IsNullOrEmpty(args.InputText), "Invalid AMOTD Entered!")) return;

                theairline.amotd = args.InputText;
                context.SaveChangesAsync();

                player.SendClientMessage(Color.SkyBlue, $"* {Color.White.ToString()}Airline ID:{aid} amotd has successfully been changed to {Color.SkyBlue.ToString()}'{args.InputText}'");
                aedit(player, context, aid);
            };
            input.Show(player);
        }
    
}
}
