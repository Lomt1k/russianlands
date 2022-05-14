using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public class InventoryItem
    {
        public int itemId { get; }
        public int itemLevel { get; private set; }
        public bool isEquipped { get; private set; }

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonConstructor]
        public InventoryItem()
        {
            data = GameDataBase.GameDataBase.instance.items[itemId];
        }

        public InventoryItem(ItemData data)
        {
            this.data = data;
            itemId = data.id;
        }

    }
}
