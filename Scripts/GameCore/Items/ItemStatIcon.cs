using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public enum ItemStatIcon : byte
    {
        PhysicalDamage,
        FireDamage,
        ColdDamage,
        LightningDamage,
        Mana,
        RestoreHealth,
        IncreaseHealth,
        KeywordSwordBlock,
        KeywordBowLastShot,
        KeywordAddArrow,
        KeywordStealMana,
        KeywordAdditionalDamage,
        KeywordRage,
        KeywordFinishing,
        KeywordAbsorption,
        KeywordStun,
        KeywordSanctions
    }

    public static class ItemStatIconExtensions
    {
        public static Emoji GetEmoji(this ItemStatIcon statIcon)
        {
            return statIcon switch
            {
                ItemStatIcon.PhysicalDamage => Emojis.StatPhysicalDamage,
                ItemStatIcon.FireDamage => Emojis.StatFireDamage,
                ItemStatIcon.ColdDamage => Emojis.StatColdDamage,
                ItemStatIcon.LightningDamage => Emojis.StatLightningDamage,
                ItemStatIcon.Mana => Emojis.StatMana,
                ItemStatIcon.RestoreHealth => Emojis.StatRestoreHealth,
                ItemStatIcon.IncreaseHealth => Emojis.StatIncreaseHealth,
                ItemStatIcon.KeywordSwordBlock => Emojis.StatKeywordSwordBlock,
                ItemStatIcon.KeywordBowLastShot => Emojis.StatKeywordBowLastShot,
                ItemStatIcon.KeywordAddArrow => Emojis.StatKeywordAddArrow,
                ItemStatIcon.KeywordStealMana => Emojis.StatKeywordStealMana,
                ItemStatIcon.KeywordAdditionalDamage => Emojis.StatKeywordAdditionalDamage,
                ItemStatIcon.KeywordRage => Emojis.StatKeywordRage,
                ItemStatIcon.KeywordFinishing => Emojis.StatKeywordFinishing,
                ItemStatIcon.KeywordAbsorption => Emojis.StatKeywordAbsorption,
                ItemStatIcon.KeywordStun => Emojis.StatKeywordStun,
                ItemStatIcon.KeywordSanctions => Emojis.StatKeywordSanctions,
                _ => Emojis.Empty
            };
        }
    }
}
