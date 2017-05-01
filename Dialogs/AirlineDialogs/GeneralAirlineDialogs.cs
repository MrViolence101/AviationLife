using core.DatabaseRelated.Models;
using core.DatabaseRelated.Models.AirlineRelated;
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
    public class GeneralAirlineDialogs
    {
        public static void Airlines(Player player, MyDbContext context)
        {
            var listOfAirlines = context.airlines.Select(x => x).ToList();
            if (listOfAirlines == null) return;

            if (processing.ValidateVariable(player, listOfAirlines.Count < 1, "There are currently no available airlines on Life Of Aviation")) return;

            var list = new ListDialog($"Airlines - {Color.LightGreen}Life Of Aviation", "Choose", "Cancel");

            foreach (var airline in listOfAirlines)
            {
                list.AddItem($"{Color.LightGreen}{airline.airlineName}");
            }

            list.Response += (sender, args) =>
            {
                if (processing.ValidateVariable(player, listOfAirlines[args.ListItem] == null, "This airline does not exist anymore")) return;

                ChosenAirlineAsync(player, context, listOfAirlines[args.ListItem].airlineID);
            };

            list.Show(player);
        }

        public static async void ChosenAirlineAsync(Player player, MyDbContext context, int aid)
        {
            var theAirline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (theAirline == null) return;

            var list = new ListDialog($"{Color.LightGreen}Airline - {theAirline.airlineName}", "Select", "Close");
            var actions = new List<Action>();


            list.AddItem("Airline Information");
            actions.Add(() =>
               ChosenAirline_Information(player, context, aid)
            );

            list.AddItem("Airline History");
            actions.Add(() =>
               ChosenAirline_History(player, context, aid)
            );


            list.AddItem("Announcements");
            actions.Add(() =>
                ChosenAirline_Announcements(player, context, aid)
            );


            list.AddItem("Employee List");
            actions.Add(() =>
                ChosenAirline_EmployeeList(player, context, aid)
            );

            list.AddItem("Online Employees");
            actions.Add(() =>
                ChosenAirline_OnlineEmployees(player, context, aid)
            );

            list.AddItem("Donations");
            actions.Add(() =>
                ChosenAirline_Donations(player, context, aid)
            );

            list.AddItem("Messages");
            actions.Add(() =>
               ChosenAirline_Messages(player, context, aid)
            );

            if (player.airlineID == null)
            {
                list.AddItem("Submit an Application");
                actions.Add(() =>
                ChosenAirline_SubmitAnApplicationAsync(player, context, aid)
                );
            }

            var e = await list.ShowAsync(player);

            if (e.DialogButton == DialogButton.Right) return;

            actions[e.ListItem]();
        }

        public static void ChosenAirline_Information(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_History(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_Announcements(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_EmployeeList(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_OnlineEmployees(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_Donations(Player player, MyDbContext context, int aid)
        {

        }

        public static void ChosenAirline_Messages(Player player, MyDbContext context, int aid)
        {

        }

        public static async void ChosenAirline_SubmitAnApplicationAsync(Player player, MyDbContext context, int aid)
        {
            var pendingApplicationForPlayer = context.pendingApplications.Include(x => x.applyingPlayer).Include(x => x.airline).FirstOrDefault(x => x.applyingPlayer.username.Equals(player.Name));
            var chosenAirline = context.airlines.FirstOrDefault(x => x.airlineID == aid);
            if (chosenAirline == null) return;

            var user = context.players.FirstOrDefault(x => x.username.Equals(player.Name));
            if (user == null) return;

            if (pendingApplicationForPlayer != null)
            {
                var msgDialog = new MessageDialog($"{Color.MediumVioletRed}Pending Application In Progress",
                                                  $"{Color.White}You already have an application submitted at " +
                                                  $"{Color.LightGreen}{pendingApplicationForPlayer.airline.airlineName}" +
                                                  $"{Environment.NewLine}{Color.White}Do you want to submit to " +
                                                  $"{Color.LightGreen}{chosenAirline.airlineName} {Color.White}instead?",
                                                  "Submit",
                                                  "Cancel");

                var e = await msgDialog.ShowAsync(player);

                if (e.DialogButton == DialogButton.Right)
                {
                        ChosenAirlineAsync(player, context, aid);
                        return;
                }

                context.pendingApplications.Remove(pendingApplicationForPlayer);

                await context.SaveChangesAsync();

                var pendingApp = new PendingAirlineApp
                {
                    airline = chosenAirline,
                    applyingPlayer = user,
                    DateApplied = DateTime.UtcNow
                };

                await context.pendingApplications.AddAsync(pendingApp);

                await context.SaveChangesAsync();

                var airlineRanks = context.ranks.Include(x => x.airline).Where(x => x.airline.airlineID == chosenAirline.airlineID).ToList();
                if (airlineRanks == null || airlineRanks.Count < 1) return;

                var playersInAirline = context.players.Include(x => x.airline).Where(x => x.airline.airlineID == aid && x.isOnline).ToList();
                if (playersInAirline == null) return;

                var authPlayersInAirline = playersInAirline.Where(x => airlineRanks[x.arank - 1].hasAppAuth).Count();

                player.SendClientMessage(Color.LightGreen, $"* {Color.White.ToString()}You successfully applied for airline {Color.LightGreen.ToString()}'{chosenAirline.airlineName}'{Color.White.ToString()}, there are {authPlayersInAirline} players online to accept your application.");

                foreach (var p in Player.GetAll<Player>())
                {
                    if (p.airlineID == chosenAirline.airlineID)
                    {
                        if (airlineRanks[p.airlineRank - 1].hasAppAuth)
                        {
                            p.SendClientMessage(Color.Orange, $"* {player.Name} {Color.White.ToString()}has just submitted an application to join the airline. {Color.Orange.ToString()}/ma {Color.White.ToString()}and accept.");
                        }
                    }
                }
            }
        }
    }
}
