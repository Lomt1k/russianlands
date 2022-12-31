using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot
{
    public enum Flag
    {
        GreatBritain,
        Russia,
    }

    public enum MenuItem
    {
        Town,
        Map,
        Buildings,
        Quests,
        Mail,
        Shop,
        Potions,
        Skills,
        Avatar,
        NameChange,
        Inventory,
        Battle,
        Premium,
    }

    public enum Stat
    {
        Health,
        Arrows,
        PhysicalDamage,
        FireDamage,
        ColdDamage,
        LightningDamage,
        RestoreHealth,
        Mana,
        IncreaseHealth,
        Premium,
        KeywordSwordBlock,
        KeywordBowLastShot,
        KeywordAddArrow,
        KeywordStealMana,
        KeywordAdditionalDamage,
        KeywordRage,
        KeywordFinishing,
        KeywordAbsorption,
        KeywordStun,
        KeywordSanctions,
    }

    public enum MapLocation
    {
        Arena,
        StoryMode,
    }

    public enum CharIcon
    {
        Male,
        MaleB,
        MaleC,
        MaleD,
        MaleE,
        MaleF,
        MaleG,
        MaleH,
        MaleI,
        Female,
        FemaleB,
        FemaleC,
        FemaleD,
        FemaleE,
        FemaleF,
        FemaleG,
        FemaleH,
        FemaleI,
        BuilderA,
        BuilderB,
        Abstract,
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
        DoubleBack,
        Info,
        Locked,
        Hourgrlass,
        BrokenHeart,
        Construction,
        LevelUp,
        Clock,
        Training,
        Cancel,
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
            { MenuItem.Quests, "\ud83d\udccc" },
            { MenuItem.Mail, "\ud83d\udceb" },
            { MenuItem.Shop, "\ud83d\uded2" },
            { MenuItem.Potions, "\ud83e\uddea" },
            { MenuItem.Skills, "\ud83d\udcaa" },
            { MenuItem.Avatar, "\ud83d\udc40" },
            { MenuItem.NameChange, "\ud83c\udff7" },
            { MenuItem.Inventory, "\ud83c\udf92" },
            { MenuItem.Battle, "\u2694\ufe0f" },
            { MenuItem.Premium, "\ud83d\udc51" },
        };

        public static readonly Dictionary<Stat, string> stats = new Dictionary<Stat, string>
        {
            { Stat.Health, "\u2764\ufe0f" },
            { Stat.Mana, "\ud83d\udd2e" },
            { Stat.Arrows, "\ud83c\udfaf" },
            { Stat.PhysicalDamage, "\ud83d\udc4a" },
            { Stat.FireDamage, "\ud83d\udd25" },
            { Stat.ColdDamage, "\ud83e\uddca" },
            { Stat.LightningDamage, "\u26a1\ufe0f" },
            { Stat.RestoreHealth, "\ud83d\udc96" },
            { Stat.IncreaseHealth, "\ud83d\udc97" },
            { Stat.Premium, "\ud83d\udc51" },
            { Stat.KeywordSwordBlock, "\ud83d\udee1" },
            { Stat.KeywordBowLastShot, "\ud83c\udf40" },
            { Stat.KeywordAddArrow, "\ud83c\udfaf" },
            { Stat.KeywordStealMana, "\ud83e\udeac" },
            { Stat.KeywordAdditionalDamage, "\ud83d\udca5" },
            { Stat.KeywordRage, "\u270a" },
            { Stat.KeywordFinishing, "\u2620\ufe0f" },
            { Stat.KeywordAbsorption, "\ud83d\udc9e" },
            { Stat.KeywordStun, "\ud83c\udf00" },
            { Stat.KeywordSanctions, "\u265f" },
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
            { ItemType.Scroll, "\ud83d\udcdc" },
            { ItemType.Any, ""},
            { ItemType.Equipped, "\ud83d\udc4b" },
        };

        public static readonly Dictionary<ResourceType, string> resources = new Dictionary<ResourceType, string>
        {
            { ResourceType.Gold, "\ud83d\udcb0" },
            { ResourceType.Diamond, "\ud83d\udc8e" },
            { ResourceType.Food, "\ud83c\udf56" },
            { ResourceType.Herbs, "\ud83c\udf3f" },
            { ResourceType.Wood, "\ud83e\udeb5" },

            { ResourceType.InventoryItems, "\ud83c\udf92" },
        };

        public static readonly Dictionary<MapLocation, string> locations = new Dictionary<MapLocation, string>
        {
            { MapLocation.Arena, "\ud83c\udfdf" },
            { MapLocation.StoryMode, "\ud83d\udea9" },
        };

        public static readonly Dictionary<CharIcon, string> characters = new Dictionary<CharIcon, string>
        {
            { CharIcon.Male, "\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb1" },
            { CharIcon.MaleB, "\ud83e\uddd1\ud83c\udffc\u200d\ud83e\uddb1" },
            { CharIcon.MaleC, "\ud83e\uddd1\ud83c\udffd" },
            { CharIcon.MaleD, "\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb0" },
            { CharIcon.MaleE, "\ud83d\udc71\ud83c\udffb" },
            { CharIcon.MaleF, "\ud83e\uddd1\ud83c\udffb" },
            { CharIcon.MaleG, "\ud83d\udc68\ud83c\udffb" },
            { CharIcon.MaleH, "\ud83d\udc68\ud83c\udffb\u200d\ud83e\uddb3" },
            { CharIcon.MaleI, "\ud83d\udc74\ud83c\udffc" },

            { CharIcon.Female, "\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb0" },
            { CharIcon.FemaleB, "\ud83d\udc69\ud83c\udffc\u200d\ud83e\uddb1" },
            { CharIcon.FemaleC, "\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb3" },
            { CharIcon.FemaleD, "\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb1" },
            { CharIcon.FemaleE, "\ud83d\udc71\ud83c\udffb\u200d\u2640\ufe0f" },
            { CharIcon.FemaleF, "\ud83d\udc69\ud83c\udffb" },
            { CharIcon.FemaleG, "\ud83d\udc69\ud83c\udffc" },
            { CharIcon.FemaleH, "\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb3" },
            { CharIcon.FemaleI, "\ud83d\udc75\ud83c\udffb" },

            { CharIcon.BuilderA, "\ud83d\udc77\ud83c\udffc\u200d\u2642\ufe0f" },
            { CharIcon.BuilderB, "\ud83d\udc77\ud83c\udffd" },
            { CharIcon.Abstract, "\ud83d\ude4e\u200d\u2642\ufe0f" },
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
            { Element.DoubleBack, "\u23ea" },
            { Element.Info, "\u2139\ufe0f" },
            { Element.Locked, "\ud83d\udd12" },
            { Element.Hourgrlass, "\u23f3" },
            { Element.BrokenHeart, "\ud83d\udc94" },
            { Element.Construction, "\u2692" },
            { Element.LevelUp, "\u23eb" },
            { Element.Clock, "\ud83d\udd53" },
            { Element.Training, "\ud83c\udf93" },
            { Element.Cancel, "\ud83d\udeab" },
        };

        public static readonly Dictionary<Smile, string> smiles = new Dictionary<Smile, string>
        {
            { Smile.Sad, "\ud83d\ude15" },
        };

    }
}
