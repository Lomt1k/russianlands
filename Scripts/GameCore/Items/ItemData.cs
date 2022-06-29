using TextGameRPG.Scripts.GameCore.GameDataBase;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Collections.Generic;
    using System.Linq;
    using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

    public class ItemData : IDataWithIntegerID
    {
        public string debugName { get; set; }
        public int id { get; }
        public ItemType itemType { get; set; }
        public Rarity itemRarity { get; set; }
        public ushort requiredLevel { get; set; }
        public byte requiredCharge { get; set; }
        public List<ItemAbilityBase> abilities { get; private set; } = new List<ItemAbilityBase>();
        public List<ItemPropertyBase> properties { get; private set; } = new List<ItemPropertyBase>();

        [JsonIgnore]
        public bool isChargeRequired => requiredCharge > 0;
        [JsonIgnore]
        public Dictionary<AbilityType, ItemAbilityBase> ablitityByType;

        [JsonIgnore]
        public Dictionary<PropertyType, ItemPropertyBase> propertyByType;

        public static ItemData brokenItem = new ItemData(ItemType.Sword, Rarity.Common, 0, 0, new List<ItemAbilityBase>(), new List<ItemPropertyBase>()) 
        { debugName = "Broken Item" };

        [JsonConstructor]
        public ItemData(string debugName, int id, ItemType type)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
        }

        // for item generator
        public ItemData(ItemType _type, Rarity _rarity, ushort _level, byte _charge,
            List<ItemAbilityBase> _abilities, List<ItemPropertyBase> _properties)
        {
            debugName = $"Generated {_type}";
            id = -1;
            itemType = _type;
            itemRarity = _rarity;
            requiredLevel = _level;
            requiredCharge = _charge;
            abilities = _abilities;
            properties = _properties;
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
