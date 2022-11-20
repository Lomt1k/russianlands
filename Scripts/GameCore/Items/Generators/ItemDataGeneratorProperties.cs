
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
