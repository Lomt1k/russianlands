using System.Collections.Generic;
using System.Linq;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Items.Generators;

public abstract partial class ItemDataGeneratorBase
{
    protected ItemDataSeed seed { get; }
    protected float gradedPoints { get; }
    protected float gradeMult { get; }

    protected Dictionary<AbilityType, ItemAbilityBase> _abilities = new Dictionary<AbilityType, ItemAbilityBase>();
    protected Dictionary<PropertyType, ItemPropertyBase> _properties = new Dictionary<PropertyType, ItemPropertyBase>();
    protected List<ItemStatIcon> _statIcons = new List<ItemStatIcon>();

    public ItemDataGeneratorBase(ItemDataSeed _seed)
    {
        seed = _seed;
        gradeMult = ((_seed.grade - 5) / 40f) + 1; //от 0.9 до 1.125 (сам grade от 1 - 10)
        gradedPoints = ItemGenerationHelper.GetBasisPoint(_seed.townHallLevel) * gradeMult;
    }

    public ItemData Generate()
    {
        GenerateItemData();
        return BakeItem();
    }

    protected abstract void GenerateItemData();

    private ItemData BakeItem()
    {
        return new ItemData(seed, _abilities.Values.ToList(), _properties.Values.ToList(), _statIcons.OrderBy(x => x).ToList());
    }

    protected void AddProperties()
    {
        foreach (var propertyType in seed.properties)
        {
            AddProperty(propertyType);
        }
    }

    protected void AddAbilities()
    {
        foreach (var abilityType in seed.abilities)
        {
            AddAbility(abilityType);
        }
    }

    //--- overriden for rings and amulets
    protected virtual void AddProperty(PropertyType propertyType)
    {
    }

    protected virtual void AddAbility(AbilityType abilityType)
    {
    }



}
