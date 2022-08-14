using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using TextGameRPG.Scripts.GameCore.Items.Generators;

    public enum ItemState : byte { IsNewAndNotEquipped = 0, IsNotEquipped = 1, IsEquipped = 2 }

    public class InventoryItem
    {
        public string id;
        public ItemState state;
        public byte mod;

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonIgnore]
        public int manaCost { get; private set; }

        [JsonIgnore]
        public bool isEquipped
        {
            get => state == ItemState.IsEquipped;
            set => state = value ? ItemState.IsEquipped : ItemState.IsNotEquipped;
        }

        [JsonConstructor]
        private InventoryItem()
        {
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RecalculateDynamicData();
        }

        public InventoryItem(int _id, byte _mod = 0)
        {
            id = _id.ToString();
            mod = _mod;
            RecalculateDynamicData();
        }

        public InventoryItem(string _dataCode, byte _mod = 0)
        {
            id = _dataCode;
            mod = _mod;
            RecalculateDynamicData();
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem()
            {
                id = id,
                mod = mod,
                state = ItemState.IsNewAndNotEquipped,
            };
            clone.RecalculateDynamicData();
            return clone;
        }

        private void RecalculateDynamicData()
        {
            data = int.TryParse(id, out int dbid)
                ? GameDataBase.GameDataBase.instance.items[dbid].Clone()
                : ItemDataDecoder.Decode(id);

            manaCost = 0;
            foreach (var ability in data.abilities)
            {
                ability.ApplyItemLevel(mod);
                manaCost += ability.manaCost;
            }
            foreach (var property in data.properties)
            {
                property.ApplyItemLevel(mod);
            }
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
            if (string.IsNullOrEmpty(data.debugName))
            {
                return Localizations.Localization.Get(session, $"item_name_{id}");
            }
            //TODO: Add localization for generated items
            return data.debugName;
        }

        public string GetFullName(GameSession session)
        {
            var sb = new StringBuilder();
            sb.Append($"{Emojis.items[data.itemType]} {GetLocalizedName(session)}");
            if (mod > 0)
            {
                sb.Append($" +{mod}");
            }

            return sb.ToString();
        }

        public bool IsSupportLevelUp()
        {
            foreach (var ability in data.abilities)
            {
                if (ability.isSupportLevelUp)
                    return true;
            }
            foreach (var property in data.properties)
            {
                if (property.isSupportLevelUp)
                    return true;
            }
            return false;
        }


    }
}
