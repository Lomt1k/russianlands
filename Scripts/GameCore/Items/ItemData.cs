using TextGameRPG.Scripts.GameCore.GameDataBase;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Collections.Generic;
    using System.Linq;

    public class ItemData : IDataWithIntegerID
    {
        public string debugName { get; set; }
        public int id { get; }
        public ItemType itemType { get; set; }
        public ItemRarity itemRarity { get; set; }
        public int requiredLevel { get; set; }
        public List<ItemPropertyBase> properties { get; private set; }

        [JsonIgnore]
        public Dictionary<ItemPropertyType, ItemPropertyBase> propertyByType;

        [JsonConstructor]
        public ItemData(string debugName, int id, ItemType type, ItemRarity rarity, int requiredLevel,
            List<ItemPropertyBase>? properties = null)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
            this.itemRarity = rarity;
            this.requiredLevel = requiredLevel;
            this.properties = properties ?? new List<ItemPropertyBase>();

            propertyByType = this.properties.ToDictionary(x => x.propertyType);
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
            return clone;
        }

        public ItemPropertyBase AddEmptyProperty()
        {
            var property = new PhysicalDamageItemProperty(0, 0);
            properties.Add(property);
            return property;
        }

        public void RemoveProperty(int index)
        {
            properties.RemoveAt(index);
        }

        public bool HasDamageProperties()
        {
            foreach (var property in properties)
            {
                switch (property.propertyType)
                {
                    case ItemPropertyType.PhysicalDamage:
                    case ItemPropertyType.FireDamage:
                    case ItemPropertyType.ColdDamage:
                    case ItemPropertyType.LightningDamage:
                        return true;
                }
            }
            return false;
        }

        public bool HasDamageResistProperties()
        {
            foreach (var property in properties)
            {
                switch (property.propertyType)
                {
                    case ItemPropertyType.PhysicalDamageResist:
                    case ItemPropertyType.FireDamageResist:
                    case ItemPropertyType.ColdDamageResist:
                    case ItemPropertyType.LightningDamageResist:
                        return true;
                }
            }
            return false;
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
                    case ItemPropertyType.PhysicalDamage:
                    case ItemPropertyType.PhysicalDamageResist:
                    case ItemPropertyType.FireDamage:
                    case ItemPropertyType.FireDamageResist:
                    case ItemPropertyType.ColdDamage:
                    case ItemPropertyType.ColdDamageResist:
                    case ItemPropertyType.LightningDamage:
                    case ItemPropertyType.LightningDamageResist:
                        continue;
                    default:
                        return true;
                }
            }
            return false;
        }

        public IEnumerable<ItemPropertyBase> GetDamageProperties()
        {
            if (propertyByType.TryGetValue(ItemPropertyType.PhysicalDamage, out var physicalDamage))
                yield return physicalDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.FireDamage, out var fireDamage))
                yield return fireDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.ColdDamage, out var coldDamage))
                yield return coldDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.LightningDamage, out var lightningDamage))
                yield return lightningDamage;
        }

        public IEnumerable<ItemPropertyBase> GetDamageResistProperties()
        {
            if (propertyByType.TryGetValue(ItemPropertyType.PhysicalDamageResist, out var physicalDamage))
                yield return physicalDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.FireDamageResist, out var fireDamage))
                yield return fireDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.ColdDamageResist, out var coldDamage))
                yield return coldDamage;
            if (propertyByType.TryGetValue(ItemPropertyType.LightningDamageResist, out var lightningDamage))
                yield return lightningDamage;
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
                    case ItemPropertyType.PhysicalDamage:
                    case ItemPropertyType.PhysicalDamageResist:
                    case ItemPropertyType.FireDamage:
                    case ItemPropertyType.FireDamageResist:
                    case ItemPropertyType.ColdDamage:
                    case ItemPropertyType.ColdDamageResist:
                    case ItemPropertyType.LightningDamage:
                    case ItemPropertyType.LightningDamageResist:
                        continue;
                    default:
                        yield return property;
                        continue;
                }
            }
        }

    }
}
