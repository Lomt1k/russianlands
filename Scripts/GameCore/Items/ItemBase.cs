using Newtonsoft.Json;
using JsonKnownTypes;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Collections.Generic;

    [JsonConverter(typeof(JsonKnownTypesConverter<ItemBase>))]
    public class ItemBase : IDataWithIntegerID
    {
        public string debugName { get; set; }
        public int id { get; }
        public ItemType itemType { get; set; }
        public ItemRarity itemRarity { get; set; }
        public int requiredLevel { get; set; }
        public List<ItemPropertyBase> properties { get; private set; }

        public ItemBase(string debugName, int id, ItemType type, ItemRarity rarity, int requiredLevel,
            List<ItemPropertyBase>? properties = null)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
            this.itemRarity = rarity;
            this.requiredLevel = requiredLevel;
            this.properties = properties ?? new List<ItemPropertyBase>();
        }

        public ItemBase Clone()
        {
            var clone = (ItemBase)MemberwiseClone();
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
    }
}
