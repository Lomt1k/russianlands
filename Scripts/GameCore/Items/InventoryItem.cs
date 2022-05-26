using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public class InventoryItem
    {
        public int itemId { get; }
        public int itemLevel { get; private set; }
        public bool isEquipped { get; private set; }
        public bool isNew { get; private set; } = true;

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonConstructor]
        private InventoryItem()
        {
            data = GameDataBase.GameDataBase.instance.items[itemId];
        }

        public InventoryItem(ItemData data)
        {
            this.data = data;
            itemId = data.id;
        }

        public InventoryItem(int id)
        {
            itemId = id;
            data = GameDataBase.GameDataBase.instance.items[itemId];
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem(data);
            clone.itemLevel = itemLevel;
            clone.isEquipped = false;
            clone.isNew = true;
            return clone;
        }

        public void SetEquippedState(bool state)
        {
            isEquipped = state;
        }

    }
}
