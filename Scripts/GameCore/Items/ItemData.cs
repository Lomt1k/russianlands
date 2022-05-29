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

            propertyByType = properties.ToDictionary(x => x.propertyType);
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

    }
}
