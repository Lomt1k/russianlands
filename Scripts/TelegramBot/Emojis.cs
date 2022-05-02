using System.Collections.Generic;

namespace TextGameRPG.Scripts.TelegramBot
{
    public enum Flag
    {
        GreatBritain,
        Russia
    }

    public enum TownMenu
    {
        Town,
        Map,
        Residents,
        Character,
        Quests,
        Mail,
        Options
    }

    public static class Emojis
    {
        public static readonly Dictionary<Flag, string> flags = new Dictionary<Flag, string>()
        {
            { Flag.GreatBritain, "\ud83c\uddec\ud83c\udde7" },
            { Flag.Russia, "\ud83c\uddf7\ud83c\uddfa" }
        };

        public static readonly Dictionary<TownMenu, string> townMenu = new Dictionary<TownMenu, string>()
        {
            { TownMenu.Town, "\ud83c\udfe0" },
            { TownMenu.Map, "\ud83d\uddfa" },
            { TownMenu.Residents, "\ud83d\udc68\u200d\ud83d\udc69\u200d\ud83d\udc67\u200d\ud83d\udc66" },
            { TownMenu.Character, "\ud83d\ude4e\u200d\u2642\ufe0f" },
            { TownMenu.Quests, "\ud83d\udccc" },
            { TownMenu.Mail, "\ud83d\udceb" },
            { TownMenu.Options, "\u2699\ufe0f" }
        };

    }
}
