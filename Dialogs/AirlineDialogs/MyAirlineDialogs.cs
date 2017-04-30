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
    public class MyAirlineDialogs
    {
        public static void MyAirline(Player player, MyDbContext context, int aid)
        {
            var list = new ListDialog("My Airline", "Edit", "Cancel");

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == aid).ToList();

            list.AddItem("Airline Information");
            list.AddItem("Airline History");
            list.AddItem("Announcements");
            list.AddItem("Employee List");
            list.AddItem("Online Employees");
            list.AddItem("Donations");
            list.AddItem("Messages");

            if (player.airlineRank == airlineRanks.Count)
            {
                list.AddItem($"Application Authorization Permissions");
            }

            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            if (airline.AppAuthRank == player.airlineRank)
            {
                var pendingApplications = context.pendingApplications.Include(x => x.airline).Include(x => x.applyingPlayer).Where(x => x.airline.airlineID == aid).ToList();
                if (pendingApplications != null && pendingApplications.Count > 0)
                {
                    list.AddItem($"{Color.White.ToString()}Pending Applications ({Color.Orange.ToString()}{pendingApplications.Count}{Color.White.ToString()})");
                }

            }
            
            

            list.Response += (sender, args) =>
            {
                switch (args.ListItem)
                {
                    case 0:
                        MyAirline_Information(player, context, aid);
                        break;
                    case 1:
                        MyAirline_History(player, context, aid);
                        break;
                    case 2:
                        MyAirline_Announcements(player, context, aid);
                        break;
                    case 3:
                        MyAirline_EmployeeList(player, context, aid);
                        break;
                    case 4:
                        MyAirline_OnlineEmployees(player, context, aid);
                        break;
                    case 5:
                        MyAirline_Donations(player, context, aid);
                        break;
                    case 6:
                        MyAirline_Messages(player, context, aid);
                        break;
                    case 8:
                        MyAirline_AppAuthPerm(player, context, aid);
                        break;
                    case 7:
                        MyAirline_PendingApplications(player, context, aid);
                        break;
                }
            };
            list.Show(player);
        }

        public static void MyAirline_AppAuthPerm(Player player, MyDbContext context, int aid)
        {
            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == aid).OrderByDescending(x => x.rankSpot).ToList();
            if (airlineRanks == null) return;
            if (airlineRanks.Count < 1) return;

            var tablist = new TablistDialog($"{Color.LightGreen}Application Authorization {Color.White}- Ranks", new string[] { "Spot", "Rank", "Auth Status" }, "Change", "Cancel");

            foreach (var rank in airlineRanks)
            {
                if (rank.hasAppAuth)
                {
                    tablist.Add(new string[] { rank.rankSpot.ToString(), rank.rankName, $"{Color.LightGreen}Yes" });
                }
                else
                {
                    tablist.Add(new string[] { rank.rankSpot.ToString(), rank.rankName, $"{Color.Red}No" });
                }
                
            }

            tablist.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;

                if (airlineRanks[args.ListItem] == null) return;
                
                if (airlineRanks[args.ListItem].hasAppAuth)
                {
                    airlineRanks[args.ListItem].hasAppAuth = false;
                }
                else
                {
                    airlineRanks[args.ListItem].hasAppAuth = true;
                }

                context.SaveChanges();
                tablist.Show(player);
            };

            tablist.Show(player);
        }
        public static void MyAirline_Messages(Player player, MyDbContext context, int aid)
        {

        }
        public static void MyAirline_Donations(Player player, MyDbContext context, int aid)
        {
            var input = new InputDialog("{FFFFFF}Donation", $"{Color.White}Thank you for your consideration to donate to the airline." +
                                                            $"{Environment.NewLine}{Color.White}Please enter a valid amount to donate " +
                                                            $"{Color.Orange}($){Color.White}:",false,"Donate","Cancel");

            input.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                try
                {
                    if (String.IsNullOrEmpty(args.InputText))
                    {
                        player.SendClientMessage(Color.Red, $"ERROR: {Color.White}You entered an invalid donation amount. Please try again.");
                        MyAirline_Donations(player, context, aid);
                        return;
                    }

                    
                    int donationAmount = Convert.ToInt32(args.InputText);

                    if (player.Money < donationAmount)
                    {
                        player.SendClientMessage(Color.Red, $"ERROR: {Color.White}You do not have sufficient cash to donate!");
                        MyAirline_Donations(player, context, aid);
                        return;
                    }

                    if (donationAmount < 1)
                    {
                        player.SendClientMessage(Color.Red, $"ERROR: {Color.White}You entered an invalid donation amount. Please try again.");
                        MyAirline_Donations(player, context, aid);
                        return;
                    }

                    var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
                    if (airline == null) return;

                    airline.bankBalance = airline.bankBalance + donationAmount;
                    context.SaveChanges();

                    player.Money = player.Money - donationAmount;
                    foreach (var p in Player.GetAll<Player>())
                    {
                        if (p.airlineID == aid)
                        {
                            p.SendClientMessage(Color.LightGreen, $"* ${Convert.ToDouble(donationAmount).ToString("N2")} {Color.White}has just been donated to the airline by {Color.LightGreen}{player.Name}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    player.SendClientMessage(Color.Red, $"ERROR: {Color.White}There was a problem processing the donation. Try to avoid decimal amounts.");
                    MyAirline_Donations(player, context, aid);
                }
            };
            input.Show(player);
        }
        public static void MyAirline_EmployeeList(Player player, MyDbContext context, int aid)
        {
            var airlineUsers = context.players.Include(x => x.airline).Where(x => x.airline.airlineID == aid).OrderByDescending(x => x.arank).ToList();
            if (airlineUsers == null) return;
            if (airlineUsers.Count < 1)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}There are no employees for this airline!");
                MyAirline(player, context, aid);
                return;
            }

            var airline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (airline == null) return;

            var ranks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == aid).ToList();
            if (ranks == null) return;
            if (ranks.Count < 1) return;

            var tablist = new TablistDialog($"My Airline - {"Employee List"}", new string[] { "Rank", "Member", "Status" }, "Ok");

            foreach (var eachUser in airlineUsers)
            {
                if (eachUser.isOnline)
                {
                    if (eachUser.arank == ranks.Count)
                    {
                        tablist.Add(new string[] { $"{Color.Red.ToString()}{ranks[eachUser.arank - 1].rankName}", eachUser.username, $"{Color.LightGreen.ToString()}Online" });
                    }
                    else
                    {
                        tablist.Add(new string[] { $"{ranks[eachUser.arank - 1].rankName}", eachUser.username, $"{Color.LightGreen.ToString()}Online" });
                    }

                }
                else
                {
                    if (eachUser.arank == ranks.Count)
                    {
                        tablist.Add(new string[] { $"{Color.Red.ToString()}{ranks[eachUser.arank - 1].rankName}", eachUser.username, $"{Color.Red.ToString()}Offline" });
                    }
                    else
                    {
                        tablist.Add(new string[] { $"{ranks[eachUser.arank - 1].rankName}", eachUser.username, $"{Color.Red.ToString()}Offline" });
                    }
                }

            }
            tablist.Show(player);

        }
        public static void MyAirline_Announcements(Player player, MyDbContext context, int aid)
        {
            var airlineAnnouncements = context.airlineAnnouncements.Include(x => x.airline).Include(x => x.createdBy).Where(x => x.airline.airlineID == aid).ToList();
            if (airlineAnnouncements == null || airlineAnnouncements.Count < 1)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}There are currently no announcements for your airline!");
                return;
            }

            string anns = "";
            anns += Environment.NewLine;
            foreach (var item in airlineAnnouncements)
            {
                anns += $"{Color.LightGreen.ToString()}[{item.DateCreated}] {Color.White.ToString()}{item.announcement} \t{Color.LightGreen.ToString()}By: {item.createdBy.username}{Environment.NewLine}";
            }
            var msg = new MessageDialog("Announcements - My Airline", anns, "Ok");
            msg.Show(player);
        }
        public static void MyAirline_PendingApplications(Player player, MyDbContext context, int aid)
        {
            var tablist = new TablistDialog("My Airline - Pending Applications", new string[] { "User", "Date Applied" }, "Accept", "Cancel");

            var pendingForAirline = context.pendingApplications.Include(x => x.airline).Include(x => x.applyingPlayer).Where(x => x.airline.airlineID == aid).ToList();
            if (processing.ValidateVariable(player, pendingForAirline == null || pendingForAirline.Count < 1, "There are not pending applications anymore!")) return;

            foreach (var item in pendingForAirline)
            {
                tablist.Add(new string[] { item.applyingPlayer.username, item.DateApplied.ToString() });
            }

            tablist.Response += (sender, args) =>
            {
                if (args.DialogButton == DialogButton.Right) return;
                if (processing.ValidateVariable(player, pendingForAirline[args.ListItem] == null, "This application does not exist anymore!")) return;

                var applyingUser = context.players.FirstOrDefault(x => x.username.Equals(pendingForAirline[args.ListItem].applyingPlayer.username));
                if (applyingUser == null) return;

                applyingUser.airline = pendingForAirline[args.ListItem].airline;
                applyingUser.arank = 1;

                bool foundUserOnServer = false;

                foreach (var p in Player.GetAll<Player>())
                {
                    if (p.Name.Equals(pendingForAirline[args.ListItem].applyingPlayer.username))
                    {
                        p.airlineID = pendingForAirline[args.ListItem].airline.airlineID;
                        p.airlineRank = 1;

                        foreach (var play in Player.GetAll<Player>())
                        {
                            if (play.airlineID == pendingForAirline[args.ListItem].airline.airlineID)
                            {
                                play.SendClientMessage(Color.LightGreen, $"* {p.Name} {Color.White.ToString()}has been accepted into the airline.");
                            }
                        }
                        foundUserOnServer = true;
                        break;
                    }
                }

                if (!foundUserOnServer)
                {
                    foreach (var play in Player.GetAll<Player>())
                    {
                        if (play.airlineID == pendingForAirline[args.ListItem].airline.airlineID)
                        {
                            play.SendClientMessage(Color.LightGreen, $"* {Color.White.ToString()}Offline Player {Color.LightGreen.ToString()}{applyingUser.username} {Color.White.ToString()}has been accepted into the airline.");
                        }
                    }
                }

                foundUserOnServer = false;
                context.pendingApplications.Remove(pendingForAirline[args.ListItem]);
                context.SaveChangesAsync();



            };
            tablist.Show(player);
        }

        public static void MyAirline_History(Player player, MyDbContext context, int aid)
        {

        }
        public static void MyAirline_OnlineEmployees(Player player, MyDbContext context, int aid)
        {
            var tablist = new TablistDialog("My Airline - Online Employees", new string[] { "Rank", "User" }, "Ok");

            var airlineUsers = context.players.Include(x => x.airline).Where(x => x.airline.airlineID == aid && x.isOnline == true).OrderByDescending(x => x.arank).ToList();
            if (airlineUsers == null || airlineUsers.Count < 1)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {Color.White.ToString()}No users of this airline are online!");
                MyAirline(player, context, aid);
                return;
            }

            var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == aid).ToList();
            if (airlineRanks == null || airlineRanks.Count < 1) return;

            foreach (var user in airlineUsers)
            {
                if (user.arank == airlineRanks.Count)
                {
                    tablist.Add(new string[] { $"{Color.Red.ToString()}{airlineRanks[user.arank-1].rankName}", user.username });
                }
                else
                {
                    tablist.Add(new string[] { $"{airlineRanks[user.arank].rankName}", user.username });
                }


            }

            tablist.Show(player);
        }




        public static void MyAirline_Information(Player player, MyDbContext context, int aid)
        {

        }
    }
}
