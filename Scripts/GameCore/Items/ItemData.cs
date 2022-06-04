using TextGameRPG.Scripts.GameCore.GameDataBase;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemData : IDataWithIntegerID
    {
        public string debugName { get; set; } //TODO: для продакшна очень важно отказаться от этого поля (как вариант при старте бота присваивать string.empty)
        public int id { get; }
        public ItemType itemType { get; set; }
        public ItemRarity itemRarity { get; set; }
        public ushort requiredLevel { get; set; }
        public List<ItemPropertyBase> properties { get; private set; }

        [JsonIgnore]
        public Dictionary<ItemPropertyType, ItemPropertyBase> propertyByType;

        [JsonConstructor]
        public ItemData(string debugName, int id, ItemType type, ItemRarity rarity, ushort requiredLevel,
            List<ItemPropertyBase>? properties = null)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
            this.itemRarity = rarity;
            this.requiredLevel = requiredLevel;
            this.properties = properties ?? new List<ItemPropertyBase>();
            RebuildPropertyByTypeDictionary();
        }

        public ItemData Clone()
        {
            var clone = (ItemData)MemberwiseClone();
            var cloneProperties = new List<ItemPropertyBase>(properties.Count);
            foreach (var property in properties)
            {
                cloneProperties.Add(property.Clone());
            }
            clone.properties = cloneProperties;
            clone.RebuildPropertyByTypeDictionary();
            return clone;
        }

        private void RebuildPropertyByTypeDictionary()
        {
            propertyByType = properties.ToDictionary(x => x.propertyType);
        }

        public ItemPropertyBase AddEmptyProperty()
        {
            var property = new DealDamageItemProperty();
            properties.Add(property);
            return property;
        }

        public void RemoveProperty(int index)
        {
            properties.RemoveAt(index);
        }

        /// <summary>
        /// Has any properties without *Damage и *DamageResist
        /// </summary>
        public bool HasSpecificProperties()
        {
            foreach (var property in properties)
            {
                switch (property.propertyType)
                {
                    case ItemPropertyType.DealDamage:
                    case ItemPropertyType.DamageResist:
                        continue;
                    default:
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Any properties without *Damage и *DamageResist
        /// </summary>
        public IEnumerable<ItemPropertyBase> GetSpecificProperties()
        {
            foreach (var property in properties)
            {
                switch (property.propertyType)
                {
                    case ItemPropertyType.DealDamage:
                    case ItemPropertyType.DamageResist:
                        continue;
                    default:
                        yield return property;
                        continue;
                }
            }
        }

    }
}
