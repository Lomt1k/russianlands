using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public class InventoryItem
    {
        public int itemId { get; set; }
        public int itemLevel { get; set; }
        public bool isEquipped { get; set; }
        public bool isNew { get; set; } = true;

        [JsonIgnore]
        public ItemData? data { get; private set; }

        [JsonConstructor]
        private InventoryItem()
        {
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
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

        public string GetView(GameSession session)
        {
            return ItemViewBuilder.Build(session, this);
        }

        public string GetLocalizedName(GameSession session)
        {
            //TODO: get name with localization
            return data.debugName;
        }

        public string GetFullName(GameSession session)
        {
            var sb = new StringBuilder();
            sb.Append($"{Emojis.items[data.itemType]} {GetLocalizedName(session)}");
            if (itemLevel > 0)
            {
                sb.Append($" +{itemLevel}");
            }

            return sb.ToString();
        }

    }
}
