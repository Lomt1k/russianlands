namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public enum AbilityType : short
    {
        None = 0,
        DealDamage = 1,
        BlockIncomingDamageEveryTurn = 2,
        RestoreHealthEveryTurn = 3,
        AddManaEveryTurn = 4,
        SwordBlockEveryTurnKeyword = 5,
        BowLastShotKeyword = 6,
        AddArrowKeyword = 7,
    }

    public static class AblilityTypeExtensions
    {
        public static ViewPriority GetPriority(this AbilityType type)
        {
            return type switch
            {
                AbilityType.None => ViewPriority.GeneralInfo,
                AbilityType.DealDamage => ViewPriority.GeneralInfo,
                AbilityType.BlockIncomingDamageEveryTurn => ViewPriority.GeneralInfo,

                AbilityType.RestoreHealthEveryTurn => ViewPriority.Passive,
                AbilityType.AddManaEveryTurn => ViewPriority.Passive,
                AbilityType.SwordBlockEveryTurnKeyword => ViewPriority.Passive,

                _ => ViewPriority.SecondoryInfo
            };
        }
    }



}
