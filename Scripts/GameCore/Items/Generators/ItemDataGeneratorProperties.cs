
namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemProperties;
    using TextGameRPG.Scripts.TelegramBot;

    public abstract partial class ItemDataGeneratorBase
    {
        protected void AddPhysicalDamageResist(int value)
        {
            if (_properties.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                ((DamageResistProperty)damageResist).physicalDamage += value;
                _statIcons.Add(Stat.PhysicalDamage);
                return;
            }

            var property = (DamageResistProperty)ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist);
            property.physicalDamage = value;
            _properties.Add(PropertyType.DamageResist, property);

            // у колец и амулетов защита от физ. урона как бонусное свойство (должно быть с иконкой)
            if (seed.itemType == ItemType.Amulet || seed.itemType == ItemType.Ring)
            {
                _statIcons.Add(Stat.PhysicalDamage);
            }
        }

        protected void AddFireDamageResist(int value)
        {
            _statIcons.Add(Stat.FireDamage);
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
            _statIcons.Add(Stat.ColdDamage);
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
            _statIcons.Add(Stat.LightningDamage);
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
            _statIcons.Add(Stat.AttributeStrength);
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
            _statIcons.Add(Stat.AttributeVitality);
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
            _statIcons.Add(Stat.AttributeSorcery);
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
            _statIcons.Add(Stat.AttributeLuck);
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
            _statIcons.Add(Stat.IncreaseHealth);
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
