using core.PlayerRelated;
using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace core
{
    public class processing
    {
        public static string adminPermissions = "You do not have sufficient permissions to use this Command!";
        public static bool ValidateVariable(Player player, bool condition, string error_message)
        {
            if (condition)
            {
                player.SendClientMessage(Color.Red, $"ERROR: {"{FFFFFF}"}{error_message}");
                return true;
            }
            return false;
        }

        public static string GetAdminStatus(int alevel)
        {
            string[] alevels = new string[]{"Moderator", "Junior Admin", "General Admin",
                                            "Senior Admin", "Head Admin", "Scripter",
                                            "Head Scripter", "Co-Owner", "Owner" };
            return alevels[alevel - 1];
        }

        public static string[] randomMessages()
        {
            return new string[] {"You can use /cmds if you want to see what commands you can use.",
                                "You can use /airlines to see what airlines are available for you to join.",
                                "You can use /ma (myairline) to do certain operations inside of the airline you are part of."};
        }
    }
}
