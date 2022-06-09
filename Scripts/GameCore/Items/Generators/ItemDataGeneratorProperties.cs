
namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    using ItemProperties;

    internal abstract partial class ItemDataGeneratorBase
    {
        protected void AddPhysicalDamageResist(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist) as DamageResistProperty;
            property.physicalDamage = value;
            _properties.Add(property);
        }

        protected void AddFireDamageResist(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist) as DamageResistProperty;
            property.fireDamage = value;
            _properties.Add(property);
        }

        protected void AddColdDamageResist(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist) as DamageResistProperty;
            property.coldDamage = value;
            _properties.Add(property);
        }

        protected void AddLightningDamageResist(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.DamageResist) as DamageResistProperty;
            property.lightningDamage = value;
            _properties.Add(property);
        }

        protected void AddIncreaseAttributeStrength(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeStrength) as IncreaseAttributeStrengthProperty;
            property.value = value;
            _properties.Add(property);
        }

        protected void AddIncreaseAttributeVitality(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeVitality) as IncreaseAttributeVitalityProperty;
            property.value = value;
            _properties.Add(property);
        }

        protected void AddIncreaseAttributeSorcery(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeSorcery) as IncreaseAttributeSorceryProperty;
            property.value = value;
            _properties.Add(property);
        }

        protected void AddIncreaseAttributeLuck(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseAttributeLuck) as IncreaseAttributeLuckProperty;
            property.value = value;
            _properties.Add(property);
        }

        protected void AddIncreaseMaxHealth(int value)
        {
            var property = ItemPropertyRegistry.GetNewProperty(PropertyType.IncreaseMaxHealth) as IncreaseMaxHealthProperty;
            property.value = value;
            _properties.Add(property);
        }

    }
}
