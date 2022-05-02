using System.Collections.Generic;

namespace TextGameRPG.Scripts.TelegramBot
{
    public enum Flag
    {
        GreatBritain,
        Russia
    }

    public enum Location
    {
        Town
    }

    public static class Emojis
    {
        public static readonly Dictionary<Flag, string> flags = new Dictionary<Flag, string>()
        {
            { Flag.GreatBritain, "\ud83c\uddec\ud83c\udde7" },
            { Flag.Russia, "\ud83c\uddf7\ud83c\uddfa" }
        };

        public static readonly Dictionary<Location, string> locations = new Dictionary<Location, string>()
        {
            { Location.Town, "\u26fa\ufe0f" }
        };

    }
}
