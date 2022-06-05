using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class ItemAbilityRegistry
    {
        private static Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>()
        {
            { AbilityType.DealDamage, new DealDamageAbility() },
            { AbilityType.RestoreHealth, new RestoreHealthAbility() },
            { AbilityType.RestoreMana, new RestoreManaAbility() },

            // every turn
            { AbilityType.BlockIncomingDamageEveryTurn, new BlockIncomingDamageEveryTurnAbility() },
        };

        public static ItemAbilityBase GetNewAbility(AbilityType type)
        {
            return _abilities[type].Clone();
        }

    }
}
