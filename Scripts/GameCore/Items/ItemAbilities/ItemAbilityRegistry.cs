using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    public class ItemAbilityRegistry
    {
        private static Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>()
        {
            { AbilityType.DealDamage, new DealDamageAbility() },

            // every turn
            { AbilityType.BlockIncomingDamageEveryTurn, new BlockIncomingDamageEveryTurnAbility() },
            { AbilityType.RestoreHealthEveryTurn, new RestoreHealthEveryTurnAbility() },
            { AbilityType.AddManaEveryTurn, new AddManaEveryTurnAbility() },
            { AbilityType.SwordBlockEveryTurnKeyword, new SwordBlockKeywordAbility() },
            { AbilityType.BowLastShotKeyword, new BowLastShotKeywordAbility() },
        };

        public static ItemAbilityBase GetNewAbility(AbilityType type)
        {
            return _abilities[type].Clone();
        }

    }
}
