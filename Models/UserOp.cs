using System;

namespace Tasky.Models
{
    public class UserOp
    {
        private static string UserName = Tasky.Properties.Settings.Default.UserName;

        public string Presentation => $"{GetGreeting()}, {UserName}!\nO que vamos fazer hoje?";

        private static string GetGreeting()
        {
            int hour = DateTime.Now.Hour;

            if (hour >= 5 && hour < 12)
                return "Bom dia";
            else if (hour >= 12 && hour < 18)
                return "Boa tarde";
            else
                return "Boa noite";
        }
    }
}
