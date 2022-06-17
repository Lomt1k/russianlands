using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.TelegramBot
{
    public enum Flag
    {
        GreatBritain,
        Russia
    }

    public enum MenuItem
    {
        Town,
        Map,
        Buildings,
        Character,
        Quests,
        Mail,
        Options,
        Avatar,
        Inventory,
        Arrows
    }

    public enum Stat
    {
        Health,
        Mana,
        PhysicalDamage,
        FireDamage,
        ColdDamage,
        LightningDamage
    }

    public enum Resource
    {
        Gold,
        Diamond,
        Food
    }

    public enum MapLocation
    {
        Arena
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
        QuestionGrey,
        Back,
        Info,
        Locked
    }

    public enum Smile
    {
        Sad
    }

    public static class Emojis
    {
        public const char space = ' ';
        public const string middleSpace = "   ";
        public const string bigSpace = "     ";

        public static readonly Dictionary<Flag, string> flags = new Dictionary<Flag, string>
        {
            { Flag.GreatBritain, "\ud83c\uddec\ud83c\udde7" },
            { Flag.Russia, "\ud83c\uddf7\ud83c\uddfa" },
        };

        public static readonly Dictionary<MenuItem, string> menuItems = new Dictionary<MenuItem, string>
        {
            { MenuItem.Town, "\ud83c\udfd8" },
            { MenuItem.Map, "\ud83d\uddfa" },
            { MenuItem.Buildings, "\ud83d\uded6" },
            { MenuItem.Character, "\ud83d\ude4e\u200d\u2642\ufe0f" },
            { MenuItem.Quests, "\ud83d\udccc" },
            { MenuItem.Mail, "\ud83d\udceb" },
            { MenuItem.Options, "\u2699\ufe0f" },
            { MenuItem.Avatar, "\ud83d\udc40" },
            { MenuItem.Inventory, "\ud83c\udf92" },
            { MenuItem.Arrows, "\ud83c\udfaf" },
        };

        public static readonly Dictionary<Stat, string> stats = new Dictionary<Stat, string>
        {
            { Stat.Health, "\u2764\ufe0f" },
            { Stat.Mana, "\ud83d\udd2e" },
            { Stat.PhysicalDamage, "\ud83d\udc4a" },
            { Stat.FireDamage, "\ud83d\udd25" },
            { Stat.ColdDamage, "\ud83e\uddca" },
            { Stat.LightningDamage, "\u26a1\ufe0f" },
        };

        public static readonly Dictionary<ItemType, string> items = new Dictionary<ItemType, string>
        {
            { ItemType.Sword, "\ud83d\udde1" },
            { ItemType.Bow, "\ud83c\udff9" },
            { ItemType.Stick, "\ud83e\ude84" },
            { ItemType.Helmet, "\ud83e\ude96" },
            { ItemType.Armor, "\ud83e\udd4b" },
            { ItemType.Boots, "\ud83e\udd7e" },
            { ItemType.Shield, "\ud83d\udee1" },
            { ItemType.Amulet, "\ud83d\udcff" },
            { ItemType.Ring, "\ud83d\udc8d" },
            { ItemType.Poison, "\ud83e\uddea" },            
            { ItemType.Scroll, "\ud83d\udcdc" },
            { ItemType.Any, ""},
            { ItemType.Equipped, "\ud83d\udc4b" },
        };

        public static readonly Dictionary<Resource, string> resources = new Dictionary<Resource, string>
        {
            { Resource.Gold, "\ud83d\udcb0" },
            { Resource.Diamond, "\ud83d\udc8e" },
            { Resource.Food, "\ud83c\udf56" },
        };

        public static readonly Dictionary<MapLocation, string> locations = new Dictionary<MapLocation, string>
        {
            { MapLocation.Arena, "\ud83c\udfdf" },
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
            { Element.QuestionGrey, "\u2754" },
            { Element.Back, "\u25c0\ufe0f" },
            { Element.Info, "\u2139\ufe0f" },
            { Element.Locked, "\ud83d\udd12" },
        };

        public static readonly Dictionary<Smile, string> smiles = new Dictionary<Smile, string>
        {
            { Smile.Sad, "\ud83d\ude15" },
        };

    }
}
