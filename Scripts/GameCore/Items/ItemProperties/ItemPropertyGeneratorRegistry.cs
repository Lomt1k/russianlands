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
                { ItemPropertyType.DealDamage, new DealDamageItemProperty() },
                { ItemPropertyType.DamageResist, new DamageResistProperty() },
                { ItemPropertyType.IncreaseAttributeStrength, new IncreaseAttributeStrengthItemProperty() },
                { ItemPropertyType.IncreaseAttributeVitality, new IncreaseAttributeVitalityItemProperty() },
                { ItemPropertyType.IncreaseAttributeSorcery, new IncreaseAttributeSorceryItemProperty() },
                { ItemPropertyType.IncreaseAttributeLuck, new IncreaseAttributeLuckItemProperty() },
                { ItemPropertyType.IncreaseMaxHealth, new IncreaseMaxHealthItemProperty() },
                { ItemPropertyType.IncreaseMaxMana, new IncreaseMaxManaProperty() }
            };
        }

        public static ItemPropertyBase GetProperty(ItemPropertyType type)
        {
            return _properties[type];
        }


    }
}
