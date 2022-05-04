using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public static class ItemPropertyRegistry
    {
        private static Dictionary<ItemPropertyType, ItemPropertyBase> _properties;

        static ItemPropertyRegistry()
        {
            _properties = new Dictionary<ItemPropertyType, ItemPropertyBase>
            {
                { ItemPropertyType.PhysicalDamage, new PhysicalDamageItemProperty(0, 0) },
            };
        }

        public static ItemPropertyBase GetProperty(ItemPropertyType type)
        {
            return _properties[type];
        }


    }
}
