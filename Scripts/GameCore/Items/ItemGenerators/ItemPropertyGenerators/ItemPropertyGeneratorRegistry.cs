using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators
{
    public static class ItemPropertyGeneratorRegistry
    {
        private static Dictionary<ItemPropertyGeneratorType, ItemPropertyGeneratorBase> _properties;

        static ItemPropertyGeneratorRegistry()
        {
            _properties = new Dictionary<ItemPropertyGeneratorType, ItemPropertyGeneratorBase>
            {
                { ItemPropertyGeneratorType.PhysicalDamage, new PhysicalDamageItemPropertyGenerator(1, 10) },
            };
        }

        public static ItemPropertyGeneratorBase GetProperty(ItemPropertyGeneratorType type)
        {
            return _properties[type];
        }


    }
}
