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
                { ItemPropertyType.PhysicalDamageResist, new PhysicalDamageResistItemProperty(0) },
                { ItemPropertyType.FireDamage, new FireDamageItemProperty(0, 0) },
                { ItemPropertyType.FireDamageResist, new FireDamageResistItemProperty(0) },
                { ItemPropertyType.ColdDamage, new ColdDamageItemProperty(0, 0) },
                { ItemPropertyType.ColdDamageResist, new ColdDamageResistItemProperty(0) },
                { ItemPropertyType.LightningDamage, new LightningDamageItemProperty(0, 0) },
                { ItemPropertyType.LightningDamageResist, new LightningDamageResistItemProperty(0) },
            };
        }

        public static ItemPropertyBase GetProperty(ItemPropertyType type)
        {
            return _properties[type];
        }


    }
}
