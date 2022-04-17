using JsonKnownTypes;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.Scripts.GameCore.Items.ItemGenerators
{
    using ItemPropertyGenerators;
    using ItemProperties;
    using System.Collections.Generic;

    [JsonConverter(typeof(JsonKnownTypesConverter<ItemGeneratorBase>))]
    public class ItemGeneratorBase : IDataWithIntegerID
    {
        public string debugName { get; set; }
        public int id { get; }
        public ItemType itemType { get; set; }
        public ItemRarity itemRarity { get; set; }
        public int requiredLevel { get; set; }
        public ItemPropertyGeneratorBase[] properties { get; private set; }

        [JsonConstructor]
        public ItemGeneratorBase(string debugName, int id, ItemType itemType, ItemRarity itemRarity, int requiredLevel, ItemPropertyGeneratorBase[] properties)
        {
            this.debugName = debugName;
            this.id = id;
            this.itemType = itemType;
            this.itemRarity = itemRarity;
            this.requiredLevel = requiredLevel;
            this.properties = properties;
        }

        public ItemGeneratorBase(string debugName, int id, ItemType itemType, ItemRarity itemRarity, int requiredLevel, ItemPropertyGeneratorBase property)
            : this(debugName, id, itemType, itemRarity, requiredLevel, new ItemPropertyGeneratorBase[] { property }) { }

        public ItemGeneratorBase(string debugName, int id, ItemType itemType, ItemRarity itemRarity, int requiredLevel)
            : this(debugName, id, itemType, itemRarity, requiredLevel, new ItemPropertyGeneratorBase[] { }) { }

        public ItemGeneratorBase Clone()
        {
            var cloneProperties = new ItemPropertyGeneratorBase[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                cloneProperties[i] = properties[i].Clone();
            }
            return new ItemGeneratorBase(debugName, id, itemType, itemRarity, requiredLevel, cloneProperties);
        }

        public ItemPropertyGeneratorBase AddEmptyProperty()
        {
            var cloneProperties = new ItemPropertyGeneratorBase[properties.Length + 1];
            for (int i = 0; i < properties.Length; i++)
            {
                cloneProperties[i] = properties[i].Clone();
            }
            cloneProperties[cloneProperties.Length - 1] = new EmptyPropertyGenerator();
            properties = cloneProperties;

            return properties[properties.Length - 1];
        }

        public void RemoveProperty(int propertyIndex)
        {
            var newProperties = new List<ItemPropertyGeneratorBase>();
            for (int i = 0; i < properties.Length; i++)
            {
                if (i == propertyIndex)
                    continue;

                newProperties.Add(properties[i]);
            }
            properties = newProperties.ToArray();
        }


        //for debug
        public void ToJSON()
        {
            var fileName = $"gameData\\item_{id}.json";
            var jsonStr = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                writer.WriteLineAsync(jsonStr);
            }
        }

        public ItemBase GenerateItem()
        {
            var itemProperties = GenerateItemProperties();
            return new ItemBase(debugName, itemType, itemRarity, id, requiredLevel, itemProperties);
        }

        private ItemPropertyBase[] GenerateItemProperties()
        {
            var resultProperties = new ItemPropertyBase[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                resultProperties[i] = properties[i].Generate();
            }
            return resultProperties;
        }


    }
}
