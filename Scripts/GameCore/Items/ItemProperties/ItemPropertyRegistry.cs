using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Items.ItemProperties
{
    public static class ItemPropertyRegistry
    {
        private static Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>
        {
            { PropertyType.DamageResist, new DamageResistProperty() },
            { PropertyType.IncreaseAttributeStrength, new IncreaseAttributeStrengthProperty() },
            { PropertyType.IncreaseAttributeVitality, new IncreaseAttributeVitalityProperty() },
            { PropertyType.IncreaseAttributeSorcery, new IncreaseAttributeSorceryProperty() },
            { PropertyType.IncreaseAttributeLuck, new IncreaseAttributeLuckProperty() },
            { PropertyType.IncreaseMaxHealth, new IncreaseMaxHealthProperty() },
            { PropertyType.IncreaseMaxMana, new IncreaseMaxManaProperty() },
        };

        public static ItemPropertyBase GetNewProperty(PropertyType type)
        {
            return _properties[type].Clone();
        }


    }
}
