using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities.Keywords;

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

            // keywords
            { AbilityType.SwordBlockEveryTurnKeyword, new SwordBlockKeywordAbility() },
            { AbilityType.BowLastShotKeyword, new BowLastShotKeywordAbility() },
            { AbilityType.AddArrowKeyword, new AddArrowKeywordAbility() },
            { AbilityType.StealManaKeyword, new StealManaKeywordAbility() },
            { AbilityType.AdditionalFireDamageKeyword, new AdditionalFireDamageKeywordAbility() },
            { AbilityType.AdditionalColdDamageKeyword, new AdditionalColdDamageKeywordAbility() },
            { AbilityType.AdditionalLightningDamageKeyword, new AdditionalLightningDamageKeywordAbility() },
            { AbilityType.RageKeyword, new RageKeywordAbility() },
            { AbilityType.FinishingKeyword, new FinishingKeywordAbility() },
        };

        public static ItemAbilityBase GetNewAbility(AbilityType type)
        {
            return _abilities[type].Clone();
        }

    }
}
