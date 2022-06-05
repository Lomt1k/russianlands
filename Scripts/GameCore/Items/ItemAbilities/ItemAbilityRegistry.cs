using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemAbilities
{
    internal class ItemAbilityRegistry
    {
        private static Dictionary<AbilityType, ItemAbilityBase> _abilities;

        static ItemAbilityRegistry()
        {
            _abilities = new Dictionary<AbilityType, ItemAbilityBase>
            {
                { AbilityType.DealDamage, new DealDamageAbility() },
            };
        }

        public static ItemAbilityBase GetNewAbility(AbilityType type)
        {
            return _abilities[type].Clone();
        }

    }
}
