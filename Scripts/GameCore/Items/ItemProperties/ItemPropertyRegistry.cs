using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public static class ItemPropertyRegistry
    {
        private static Dictionary<PropertyType, ItemPropertyBase> _properties;

        static ItemPropertyRegistry()
        {
            _properties = new Dictionary<PropertyType, ItemPropertyBase>
            {
                { PropertyType.DamageResist, new DamageResistProperty() },
                { PropertyType.IncreaseAttributeStrength, new IncreaseAttributeStrengthItemProperty() },
                { PropertyType.IncreaseAttributeVitality, new IncreaseAttributeVitalityItemProperty() },
                { PropertyType.IncreaseAttributeSorcery, new IncreaseAttributeSorceryItemProperty() },
                { PropertyType.IncreaseAttributeLuck, new IncreaseAttributeLuckItemProperty() },
                { PropertyType.IncreaseMaxHealth, new IncreaseMaxHealthItemProperty() },
                { PropertyType.IncreaseMaxMana, new IncreaseMaxManaProperty() }
            };
        }

        public static ItemPropertyBase GetNewProperty(PropertyType type)
        {
            return _properties[type].Clone();
        }


    }
}
