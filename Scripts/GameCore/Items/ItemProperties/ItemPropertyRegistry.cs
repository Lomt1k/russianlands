using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public static class ItemPropertyRegistry
    {
        private static Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>
        {
            { PropertyType.DamageResist, new DamageResistProperty() },
            { PropertyType.IncreaseMaxHealth, new IncreaseMaxHealthProperty() },
        };

        public static ItemPropertyBase GetNewProperty(PropertyType type)
        {
            return _properties[type].Clone();
        }


    }
}
