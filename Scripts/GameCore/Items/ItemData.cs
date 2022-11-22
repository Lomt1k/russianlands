using TextGameRPG.Scripts.GameCore.GameDataBase;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Collections.Generic;
    using System.Linq;
    using TextGameRPG.Scripts.GameCore.Items.Generators;
    using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
    using TextGameRPG.Scripts.Bot;

    public class ItemData : IDataWithIntegerID
    {
        public string debugName { get; set; }
        public int id { get; }
        public ItemType itemType { get; set; }
        public Rarity itemRarity { get; set; }
        public byte requiredLevel { get; set; }
        public byte requiredCharge { get; set; }
        public List<ItemAbilityBase> abilities { get; private set; } = new List<ItemAbilityBase>();
        public List<ItemPropertyBase> properties { get; private set; } = new List<ItemPropertyBase>();
        public List<Stat> statIcons { get; private set; } = new List<Stat>();

        [JsonIgnore]
        public byte requiredTownHall { get; }
        [JsonIgnore]
        public byte grade { get; }
        [JsonIgnore]
        public bool isChargeRequired => requiredCharge > 0;
        [JsonIgnore]
        public bool isGeneratedItem => id == -1;
        [JsonIgnore]
        public Dictionary<AbilityType, ItemAbilityBase> ablitityByType;

        [JsonIgnore]
        public Dictionary<PropertyType, ItemPropertyBase> propertyByType;

        public static ItemData brokenItem = new ItemData(new ItemDataSeed(), new List<ItemAbilityBase>(), new List<ItemPropertyBase>(), new List<Stat>()) 
        { debugName = "Broken Item" };

        [JsonConstructor]
        public ItemData(string debugName, int id, ItemType type)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
        }

        // from item generator
        public ItemData(ItemDataSeed _seed, List<ItemAbilityBase> _abilities, List<ItemPropertyBase> _properties, List<Stat> _statIcons)
        {
            debugName = string.Empty;
            id = -1;
            itemType = _seed.itemType;
            itemRarity = _seed.rarity;
            requiredLevel = _seed.requiredLevel;
            requiredCharge = _seed.requiredCharge;
            requiredTownHall = _seed.townHallLevel;
            grade = _seed.grade;
            abilities = _abilities;
            properties = _properties;
            statIcons = _statIcons;
            RebuildDictionaries();
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

        public void OnSetupAppMode(AppMode appMode)
        {
            if (appMode == AppMode.Bot)
            {
                debugName = string.Empty;
            }
        }

    }
}
