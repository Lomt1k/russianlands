
namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemProperties;

    public abstract partial class ItemDataGeneratorBase
    {
        protected void AddPhysicalDamageResist(int value)
        {
            if (_properties.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                ((DamageResistProperty)damageResist).physicalDamage += value;
                return;
            }

            var property = (DamageResistProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist);
            property.physicalDamage = value;
            _properties.Add(PropertyType.DamageResist, property);
        }

        protected void AddFireDamageResist(int value)
        {
            if (_properties.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                ((DamageResistProperty)damageResist).fireDamage += value;
                return;
            }

            var property = (DamageResistProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist);
            property.fireDamage = value;
            _properties.Add(PropertyType.DamageResist, property);
        }

        protected void AddColdDamageResist(int value)
        {
            if (_properties.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                ((DamageResistProperty)damageResist).coldDamage += value;
                return;
            }

            var property = (DamageResistProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist);
            property.coldDamage = value;
            _properties.Add(PropertyType.DamageResist, property);
        }

        protected void AddLightningDamageResist(int value)
        {
            if (_properties.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                ((DamageResistProperty)damageResist).lightningDamage += value;
                return;
            }

            var property = (DamageResistProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist);
            property.lightningDamage = value;
            _properties.Add(PropertyType.DamageResist, property);
        }

        protected void AddIncreaseAttributeStrength(int value)
        {
            if (_properties.TryGetValue(PropertyType.IncreaseAttributeStrength, out var property))
            {
                ((IncreaseAttributeStrengthProperty)property).value += value;
                return;
            }

            var newProperty = (IncreaseAttributeStrengthProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeStrength);
            newProperty.value = value;
            _properties.Add(PropertyType.IncreaseAttributeStrength, newProperty);
        }

        protected void AddIncreaseAttributeVitality(int value)
        {
            if (_properties.TryGetValue(PropertyType.IncreaseAttributeVitality, out var property))
            {
                ((IncreaseAttributeVitalityProperty)property).value += value;
                return;
            }

            var newProperty = (IncreaseAttributeVitalityProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeVitality);
            newProperty.value = value;
            _properties.Add(PropertyType.IncreaseAttributeVitality, newProperty);
        }

        protected void AddIncreaseAttributeSorcery(int value)
        {
            if (_properties.TryGetValue(PropertyType.IncreaseAttributeSorcery, out var property))
            {
                ((IncreaseAttributeSorceryProperty)property).value += value;
                return;
            }

            var newProperty = (IncreaseAttributeSorceryProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeSorcery);
            newProperty.value = value;
            _properties.Add(PropertyType.IncreaseAttributeSorcery, newProperty);
        }

        protected void AddIncreaseAttributeLuck(int value)
        {
            if (_properties.TryGetValue(PropertyType.IncreaseAttributeLuck, out var property))
            {
                ((IncreaseAttributeLuckProperty)property).value += value;
                return;
            }

            var newProperty = (IncreaseAttributeLuckProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeLuck);
            newProperty.value = value;
            _properties.Add(PropertyType.IncreaseAttributeLuck, newProperty);
        }

        protected void AddIncreaseMaxHealth(int value)
        {
            if (_properties.TryGetValue(PropertyType.IncreaseMaxHealth, out var property))
            {
                ((IncreaseMaxHealthProperty)property).value += value;
                return;
            }

            var newProperty = (IncreaseMaxHealthProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseMaxHealth);
            newProperty.value = value;
            _properties.Add(PropertyType.IncreaseMaxHealth, newProperty);
        }

    }
}
