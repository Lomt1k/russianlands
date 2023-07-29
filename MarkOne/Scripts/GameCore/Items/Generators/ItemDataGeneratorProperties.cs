using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Items.Generators;

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
