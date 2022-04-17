using Newtonsoft.Json;
using JsonKnownTypes;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;

    [JsonConverter(typeof(JsonKnownTypesConverter<ItemBase>))]
    public class ItemBase : IDataWithIntegerID
    {
        public string debugName { get; }
        public int id { get; }
        public ItemType itemType { get; }
        public ItemRarity itemRarity { get; }
        public int requiredLevel { get; }
        public ItemPropertyBase[] properties { get; }

        public ItemBase(string debugName, ItemType type, ItemRarity rarity, int id, int requiredLevel, ItemPropertyBase[] properties)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = type;
            this.itemRarity = rarity;
            this.requiredLevel = requiredLevel;
            this.properties = properties;
        }

    }
}
