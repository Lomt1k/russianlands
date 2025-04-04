﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MarkOne.Scripts.GameCore.Items.Generators;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemProperties;
using MarkOne.Scripts.GameCore.Services.GameData;
using System;

namespace MarkOne.Scripts.GameCore.Items;

public class ItemData : IGameDataWithId<int>
{
    public string debugName { get; set; }
    public int id { get; }
    public ItemType itemType { get; set; }
    public Rarity itemRarity { get; set; }
    public byte requiredLevel { get; set; }
    public List<ItemAbilityBase> abilities { get; private set; } = new List<ItemAbilityBase>();
    public List<ItemPropertyBase> properties { get; private set; } = new List<ItemPropertyBase>();
    public ItemStatIcon[] statIcons { get; private set; } = Array.Empty<ItemStatIcon>();

    [JsonIgnore]
    public byte requiredTownHall { get; private set; }
    [JsonIgnore]
    public byte grade { get; }
    [JsonIgnore]
    public bool isGeneratedItem => id == -1;
    [JsonIgnore]
    public Dictionary<AbilityType, ItemAbilityBase> ablitityByType;

    [JsonIgnore]
    public Dictionary<PropertyType, ItemPropertyBase> propertyByType;

    public static ItemData brokenItem = new ItemData(new ItemDataSeed(), new List<ItemAbilityBase>(), new List<ItemPropertyBase>())
    { debugName = "Broken Item" };

    [JsonConstructor]
    public ItemData(string debugName, int id, ItemType type)
    {
        this.debugName = debugName;
        this.id = id;
        this.itemType = type;
    }

    // from item generator
    public ItemData(ItemDataSeed _seed, List<ItemAbilityBase> _abilities, List<ItemPropertyBase> _properties)
    {
        debugName = string.Empty;
        id = -1;
        itemType = _seed.itemType;
        itemRarity = _seed.rarity;
        requiredLevel = _seed.requiredLevel;
        requiredTownHall = _seed.townHallLevel;
        grade = _seed.grade;
        abilities = _abilities;
        properties = _properties;
        RebuildDictionaries();
    }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        requiredTownHall = CalculateTownhallByLevel();
    }

    public ItemData Clone()
    {
        var clone = (ItemData)MemberwiseClone();

        var cloneAbilities = new List<ItemAbilityBase>(abilities.Count);
        foreach (var ability in abilities)
        {
            cloneAbilities.Add(ability.Clone());
        }
        clone.abilities = cloneAbilities;

        var cloneProperties = new List<ItemPropertyBase>(properties.Count);
        foreach (var property in properties)
        {
            cloneProperties.Add(property.Clone());
        }
        clone.properties = cloneProperties;

        clone.RebuildDictionaries();
        return clone;
    }

    private void RebuildDictionaries()
    {
        ablitityByType = abilities.ToDictionary(x => x.abilityType);
        propertyByType = properties.ToDictionary(x => x.propertyType);

        // rerfresh statIcons
        var iconsList = new List<ItemStatIcon>();
        foreach (var ability in abilities)
        {
            iconsList.AddRange(ability.GetIcons(itemType));
        }
        foreach (var ability in properties)
        {
            iconsList.AddRange(ability.GetIcons(itemType));
        }
        statIcons = iconsList.OrderBy(x => x).ToArray();
    }

    public ItemAbilityBase AddEmptyAbility()
    {
        var ability = new DealDamageAbility();
        abilities.Add(ability);
        return ability;
    }

    public ItemPropertyBase AddEmptyProperty()
    {
        var property = new DamageResistProperty();
        properties.Add(property);
        return property;
    }

    public void RemoveAbility(int index)
    {
        abilities.RemoveAt(index);
    }

    public void RemoveProperty(int index)
    {
        properties.RemoveAt(index);
    }

    public void OnBotAppStarted()
    {
        debugName = string.Empty;
    }

    private byte CalculateTownhallByLevel()
    {
        if (requiredLevel >= 29) return 8;
        if (requiredLevel >= 21) return 7;
        if (requiredLevel >= 16) return 6;
        if (requiredLevel >= 11) return 5;
        if (requiredLevel >= 8) return 4;
        if (requiredLevel >= 5) return 3;
        return 2;
    }

}
