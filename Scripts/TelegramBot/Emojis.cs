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
        Buildings,
        Character,
        Quests,
        Mail,
        Options
    }

    public enum Attribute
    {
        Health,
        Mana
    }

    public enum Resource
    {
        Gold,
        Diamond,
        Food
    }

    public enum Element
    {
        SmallBlack,
        SmallWhite,
        MediumBlack,
        MediumWhite,
        BigBlack,
        BigWhite,
        LargeBlack,
        LargeWhite,
        TriangleUp,
        TriangleDown,
        CheckmarkGreen,
        CheckmarkBlack,
        CrossGreen,
        CrossBlack,
        CrossRed,
        Plus,
        Minus,
        Infinity,
        Warning,
        WarningRed,
        WarningGrey,
        QuestionRed,
        QuestionGrey
    }

    public static class Emojis
    {
        public const string space = "     ";

        public static readonly Dictionary<Flag, string> flags = new Dictionary<Flag, string>
        {
            { Flag.GreatBritain, "\ud83c\uddec\ud83c\udde7" },
            { Flag.Russia, "\ud83c\uddf7\ud83c\uddfa" }
        };

        public static readonly Dictionary<TownMenu, string> townMenu = new Dictionary<TownMenu, string>
        {
            { TownMenu.Town, "\ud83c\udfd8" },
            { TownMenu.Map, "\ud83d\uddfa" },
            { TownMenu.Buildings, "\ud83d\uded6" },
            { TownMenu.Character, "\ud83d\ude4e\u200d\u2642\ufe0f" },
            { TownMenu.Quests, "\ud83d\udccc" },
            { TownMenu.Mail, "\ud83d\udceb" },
            { TownMenu.Options, "\u2699\ufe0f" }
        };

        public static readonly Dictionary<Attribute, string> atributes = new Dictionary<Attribute, string>
        {
            { Attribute.Health, "\u2764\ufe0f" },
            { Attribute.Mana, "\ud83d\udd2e" }
        };

        public static readonly Dictionary<Resource, string> resources = new Dictionary<Resource, string>
        {
            { Resource.Gold, "\ud83d\udcb0" },
            { Resource.Diamond, "\ud83d\udc8e" },
            { Resource.Food, "\ud83c\udf56" }
        };

        public static readonly Dictionary<Element, string> elements = new Dictionary<Element, string>
        {
            { Element.SmallBlack, "\u25aa\ufe0f" },
            { Element.SmallWhite, "\u25ab\ufe0f" },
            { Element.MediumBlack, "\u25fe\ufe0f" },
            { Element.MediumWhite, "\u25fd\ufe0f" },
            { Element.BigBlack, "\u25fc\ufe0f" },
            { Element.BigWhite, "\u25fb\ufe0f" },
            { Element.LargeBlack, "\u2b1b\ufe0f" },
            { Element.LargeWhite, "\u2b1c\ufe0f" },
            { Element.TriangleUp, "\ud83d\udd3a" },
            { Element.TriangleDown, "\ud83d\udd3b" },
            { Element.CheckmarkGreen, "\u2705" },
            { Element.CheckmarkBlack, "\u2611\ufe0f" },
            { Element.CrossGreen, "\u274e" },
            { Element.CrossBlack, "\u2716\ufe0f" },
            { Element.CrossRed, "\u274c" },
            { Element.Plus, "\u2795" },
            { Element.Minus, "\u2796" },
            { Element.Infinity, "\u267e" },
            { Element.Warning, "\u26a0\ufe0f" },
            { Element.WarningRed, "\u2757\ufe0f" },
            { Element.WarningGrey, "\u2755" },
            { Element.QuestionRed, "\u2753" },
            { Element.QuestionGrey, "\u2754" }
        };

    }
}
