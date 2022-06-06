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
        public string debugName { get; set; } //TODO: для продакшна очень важно отказаться от этого поля (как вариант при старте бота присваивать string.empty)
        public int id { get; }
        public ItemType itemType { get; set; }
        public ItemRarity itemRarity { get; set; }
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

        [JsonConstructor]
        public ItemData(string debugName, int id, ItemType type)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
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

    }
}
