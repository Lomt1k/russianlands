namespace TextGameRPG.Scripts.Bot
{
    /* Important info:
     * text + emoji => "text [emoji]"
     * text + emoji.ToString() => "text[emoji]" (without space)
     */
    public class Emoji
    {
        private string _code { get; }

        public Emoji(string code)
        {
            _code = code;
        }

        public override string ToString()
        {
            return _code;
        }

        public static string operator +(Emoji a, string b)
        {
            return a.ToString() + ' ' + b;
        }

        public static string operator +(string a, Emoji b)
        {
            return a + ' ' + b.ToString();
        }
    }

    public static class Emojis
    {
        public const string middleSpace = "   ";
        public const string bigSpace = "     ";

        public static readonly Emoji Empty = new Emoji(string.Empty);

        // --- Flags
        public static readonly Emoji FlagBritain = new Emoji("\ud83c\uddec\ud83c\udde7");
        public static readonly Emoji FlagRussia = new Emoji("\ud83c\uddf7\ud83c\uddfa");

        // --- Buttons
        public static readonly Emoji ButtonTown = new Emoji("\ud83c\udfd8");
        public static readonly Emoji ButtonMap = new Emoji("\ud83d\uddfa");
        public static readonly Emoji ButtonBuildings = new Emoji("\ud83d\uded6");
        public static readonly Emoji ButtonQuests = new Emoji("\ud83d\udccc");
        public static readonly Emoji ButtonMail = new Emoji("\ud83d\udceb");
        public static readonly Emoji ButtonShop = new Emoji("\ud83d\uded2");
        public static readonly Emoji ButtonPotions = new Emoji("\ud83e\uddea");
        public static readonly Emoji ButtonSkills = new Emoji("\ud83d\udcaa");
        public static readonly Emoji ButtonCraft = new Emoji("\ud83d\udd28");
        public static readonly Emoji ButtonAvatar = new Emoji("\ud83d\udc40");
        public static readonly Emoji ButtonNameChange = new Emoji("\ud83c\udff7");
        public static readonly Emoji ButtonInventory = new Emoji("\ud83c\udf92");
        public static readonly Emoji ButtonBattle = new Emoji("\u2694\ufe0f");
        public static readonly Emoji ButtonArena = new Emoji("\ud83c\udfdf");
        public static readonly Emoji ButtonStoryMode = new Emoji("\ud83d\udea9");

        // --- Stats
        public static readonly Emoji StatHealth = new Emoji("\u2764\ufe0f");
        public static readonly Emoji StatMana = new Emoji("\ud83d\udd2e");
        public static readonly Emoji StatArrows = new Emoji("\ud83c\udfaf");
        public static readonly Emoji StatPhysicalDamage = new Emoji("\ud83d\udc4a");
        public static readonly Emoji StatFireDamage = new Emoji("\ud83d\udd25");
        public static readonly Emoji StatColdDamage = new Emoji("\ud83e\uddca");
        public static readonly Emoji StatLightningDamage = new Emoji("\u26a1\ufe0f");
        public static readonly Emoji StatRestoreHealth = new Emoji("\ud83d\udc96");
        public static readonly Emoji StatIncreaseHealth = new Emoji("\ud83d\udc97");
        public static readonly Emoji StatPremium = new Emoji("\ud83d\udc51");
        public static readonly Emoji StatKeywordSwordBlock = new Emoji("\ud83d\udee1");
        public static readonly Emoji StatKeywordBowLastShot = new Emoji("\ud83c\udf40");
        public static readonly Emoji StatKeywordAddArrow = new Emoji("\ud83c\udfaf");
        public static readonly Emoji StatKeywordStealMana = new Emoji("\ud83e\udeac");
        public static readonly Emoji StatKeywordAdditionalDamage = new Emoji("\ud83d\udca5");
        public static readonly Emoji StatKeywordRage = new Emoji("\u270a");
        public static readonly Emoji StatKeywordFinishing = new Emoji("\u2620\ufe0f");
        public static readonly Emoji StatKeywordAbsorption = new Emoji("\ud83d\udc9e");
        public static readonly Emoji StatKeywordStun = new Emoji("\ud83c\udf00");
        public static readonly Emoji StatKeywordSanctions = new Emoji("\u265f");

        // --- Items
        public static readonly Emoji ItemSword = new Emoji("\ud83d\udde1");
        public static readonly Emoji ItemBow = new Emoji("\ud83c\udff9");
        public static readonly Emoji ItemStick = new Emoji("\ud83e\ude84");
        public static readonly Emoji ItemHelmet = new Emoji("\ud83e\ude96");
        public static readonly Emoji ItemArmor = new Emoji("\ud83e\udd4b");
        public static readonly Emoji ItemBoots = new Emoji("\ud83e\udd7e");
        public static readonly Emoji ItemShield = new Emoji("\ud83d\udee1");
        public static readonly Emoji ItemAmulet = new Emoji("\ud83d\udcff");
        public static readonly Emoji ItemRing = new Emoji("\ud83d\udc8d");
        public static readonly Emoji ItemScroll = new Emoji("\ud83d\udcdc");
        public static readonly Emoji ItemEquipped = new Emoji("\ud83d\udc4b");

        // --- Resources
        public static readonly Emoji ResourceGold = new Emoji("\ud83d\udcb0");
        public static readonly Emoji ResourceDiamond = new Emoji("\ud83d\udc8e");
        public static readonly Emoji ResourceFood = new Emoji("\ud83c\udf56");
        public static readonly Emoji ResourceHerbs = new Emoji("\ud83c\udf3f");
        public static readonly Emoji ResourceWood = new Emoji("\ud83e\udeb5");
        
        public static readonly Emoji ResourceInventoryItems = new Emoji("\ud83c\udf92");
        public static readonly Emoji ResourceCraftPiecesCommon = new Emoji("\ud83d\udce6");
        public static readonly Emoji ResourceCraftPiecesRare = new Emoji("\ud83d\udce6");
        public static readonly Emoji ResourceCraftPiecesEpic = new Emoji("\ud83d\udce6");
        public static readonly Emoji ResourceCraftPiecesLegendary = new Emoji("\ud83d\udce6");

        public static readonly Emoji ResourceFruitApple = new Emoji("\ud83c\udf4e");
        public static readonly Emoji ResourceFruitPear = new Emoji("\ud83c\udf50");
        public static readonly Emoji ResourceFruitMandarin = new Emoji("\ud83c\udf4a");
        public static readonly Emoji ResourceFruitCoconut = new Emoji("\ud83e\udd65");
        public static readonly Emoji ResourceFruitPineapple = new Emoji("\ud83c\udf4d");
        public static readonly Emoji ResourceFruitBanana = new Emoji("\ud83c\udf4c");
        public static readonly Emoji ResourceFruitWatermelon = new Emoji("\ud83c\udf49");
        public static readonly Emoji ResourceFruitStrawberry = new Emoji("\ud83c\udf53");
        public static readonly Emoji ResourceFruitBlueberry = new Emoji("\ud83e\uded0");
        public static readonly Emoji ResourceFruitKiwi = new Emoji("\ud83e\udd5d");
        public static readonly Emoji ResourceFruitCherry = new Emoji("\ud83c\udf52");
        public static readonly Emoji ResourceFruitGrape = new Emoji("\ud83c\udf47");

        // --- Characters
        public static readonly Emoji AvatarMale = new Emoji("\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb1");
        public static readonly Emoji AvatarMaleB = new Emoji("\ud83e\uddd1\ud83c\udffc\u200d\ud83e\uddb1");
        public static readonly Emoji AvatarMaleC = new Emoji("\ud83e\uddd1\ud83c\udffd");
        public static readonly Emoji AvatarMaleD = new Emoji("\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb0");
        public static readonly Emoji AvatarMaleE = new Emoji("\ud83d\udc71\ud83c\udffb");
        public static readonly Emoji AvatarMaleF = new Emoji("\ud83e\uddd1\ud83c\udffb");
        public static readonly Emoji AvatarMaleG = new Emoji("\ud83d\udc68\ud83c\udffb");
        public static readonly Emoji AvatarMaleH = new Emoji("\ud83d\udc68\ud83c\udffb\u200d\ud83e\uddb3");
        public static readonly Emoji AvatarMaleI = new Emoji("\ud83d\udc74\ud83c\udffc");

        public static readonly Emoji AvatarFemale = new Emoji("\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb0");
        public static readonly Emoji AvatarFemaleB = new Emoji("\ud83d\udc69\ud83c\udffc\u200d\ud83e\uddb1");
        public static readonly Emoji AvatarFemaleC = new Emoji("\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb3");
        public static readonly Emoji AvatarFemaleD = new Emoji("\ud83d\udc69\ud83c\udffb\u200d\ud83e\uddb1");
        public static readonly Emoji AvatarFemaleE = new Emoji("\ud83d\udc71\ud83c\udffb\u200d\u2640\ufe0f");
        public static readonly Emoji AvatarFemaleF = new Emoji("\ud83d\udc69\ud83c\udffb");
        public static readonly Emoji AvatarFemaleG = new Emoji("\ud83d\udc69\ud83c\udffc");
        public static readonly Emoji AvatarFemaleH = new Emoji("\ud83e\uddd1\ud83c\udffb\u200d\ud83e\uddb3");
        public static readonly Emoji AvatarFemaleI = new Emoji("\ud83d\udc75\ud83c\udffb");

        public static readonly Emoji AvatarBuilderA = new Emoji("\ud83d\udc77\ud83c\udffc\u200d\u2642\ufe0f");
        public static readonly Emoji AvatarBuilderB = new Emoji("\ud83d\udc77\ud83c\udffd");
        public static readonly Emoji AvatarAbstract = new Emoji("\ud83d\ude4e\u200d\u2642\ufe0f");

        // --- Smiles
        public static readonly Emoji SmileSad = new Emoji("\ud83d\ude15");
        public static readonly Emoji SmileCongratulations = new Emoji("\ud83e\udd73");

        // --- Numeric
        public static readonly Emoji Number0 = new Emoji("0\ufe0f\u20e3");
        public static readonly Emoji Number1 = new Emoji("1\ufe0f\u20e3");
        public static readonly Emoji Number2 = new Emoji("2\ufe0f\u20e3");
        public static readonly Emoji Number3 = new Emoji("3\ufe0f\u20e3");
        public static readonly Emoji Number4 = new Emoji("4\ufe0f\u20e3");
        public static readonly Emoji Number5 = new Emoji("5\ufe0f\u20e3");
        public static readonly Emoji Number6 = new Emoji("6\ufe0f\u20e3");
        public static readonly Emoji Number7 = new Emoji("7\ufe0f\u20e3");
        public static readonly Emoji Number8 = new Emoji("8\ufe0f\u20e3");
        public static readonly Emoji Number9 = new Emoji("9\ufe0f\u20e3");
        public static readonly Emoji Number10 = new Emoji("\ud83d\udd1f");
        public static readonly Emoji NumberOver10 = new Emoji("*\ufe0f\u20e3");

        // --- Other elements
        public static readonly Emoji ElementSmallBlack = new Emoji("\u25aa\ufe0f");
        public static readonly Emoji ElementSmallWhite = new Emoji("\u25ab\ufe0f");
        public static readonly Emoji ElementMediumBlack = new Emoji("\u25fe\ufe0f");
        public static readonly Emoji ElementMediumWhite = new Emoji("\u25fd\ufe0f");
        public static readonly Emoji ElementBigBlack = new Emoji("\u25fc\ufe0f");
        public static readonly Emoji ElementBigWhite = new Emoji("\u25fb\ufe0f");
        public static readonly Emoji ElementLargeBlack = new Emoji("\u2b1b\ufe0f");
        public static readonly Emoji ElementLargeWhite = new Emoji("\u2b1c\ufe0f");
        public static readonly Emoji ElementTriangleUp = new Emoji("\ud83d\udd3a");
        public static readonly Emoji ElementTriangleDown = new Emoji("\ud83d\udd3b");
        public static readonly Emoji ElementCheckmarkGreen = new Emoji("\u2705");
        public static readonly Emoji ElementCheckmarkBlack = new Emoji("\u2611\ufe0f");
        public static readonly Emoji ElementCrossGreen = new Emoji("\u274e");
        public static readonly Emoji ElementCrossBlack = new Emoji("\u2716\ufe0f");
        public static readonly Emoji ElementCrossRed = new Emoji("\u274c");
        public static readonly Emoji ElementPlus = new Emoji("\u2795");
        public static readonly Emoji ElementMinus = new Emoji("\u2796");
        public static readonly Emoji ElementInfinity = new Emoji("\u267e");
        public static readonly Emoji ElementWarning = new Emoji("\u26a0\ufe0f");
        public static readonly Emoji ElementWarningRed = new Emoji("\u2757\ufe0f");
        public static readonly Emoji ElementWarningGrey = new Emoji("\u2755");
        public static readonly Emoji ElementQuestionRed = new Emoji("\u2753");
        public static readonly Emoji ElementQuestionGrey = new Emoji("\u2754");
        public static readonly Emoji ElementBack = new Emoji("\u25c0\ufe0f");
        public static readonly Emoji ElementDoubleBack = new Emoji("\u23ea");
        public static readonly Emoji ElementInfo = new Emoji("\u2139\ufe0f");
        public static readonly Emoji ElementLocked = new Emoji("\ud83d\udd12");
        public static readonly Emoji ElementHourgrlass = new Emoji("\u23f3");
        public static readonly Emoji ElementBrokenHeart = new Emoji("\ud83d\udc94");
        public static readonly Emoji ElementConstruction = new Emoji("\u2692");
        public static readonly Emoji ElementLevelUp = new Emoji("\u23eb");
        public static readonly Emoji ElementClock = new Emoji("\ud83d\udd53");
        public static readonly Emoji ElementTraining = new Emoji("\ud83c\udf93");
        public static readonly Emoji ElementCancel = new Emoji("\ud83d\udeab");
        public static readonly Emoji ElementBin = new Emoji("\ud83d\uddd1");

        public static Emoji GetNumeric(int value)
        {
            return value switch
            {
                0 => Number0,
                1 => Number1,
                2 => Number2,
                3 => Number3,
                4 => Number4,
                5 => Number5,
                6 => Number6,
                7 => Number7,
                8 => Number8,
                9 => Number9,
                10 => Number10,
                _ => NumberOver10
            };
        }

    }
}
