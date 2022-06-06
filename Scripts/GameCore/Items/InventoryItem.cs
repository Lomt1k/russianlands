using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemAbilities;

    public class InventoryItem
    {
        public int itemId;
        public byte itemLevel;
        public byte charge;
        public bool isEquipped;
        public bool isNew = true;

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonIgnore]
        public int manaCost { get; private set; }

        [JsonIgnore]
        public bool canBeActivated { get; private set; }

        [JsonConstructor]
        private InventoryItem()
        {
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RecalculateDynamicData();
        }

        public InventoryItem(int id, byte level = 0)
        {
            itemId = id;
            itemLevel = level;
            RecalculateDynamicData();
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem()
            {
                itemId = itemId,
                itemLevel = itemLevel,
                isEquipped = false,
                isNew = true,
            };
            clone.RecalculateDynamicData();
            return clone;
        }

        private void RecalculateDynamicData()
        {
            data = GameDataBase.GameDataBase.instance.items[itemId].Clone();
            manaCost = 0;
            canBeActivated = false;
            foreach (var ability in data.abilities)
            {
                ability.ApplyItemLevel(itemLevel);
                manaCost += ability.manaCost;
                if (ability.activationType == ActivationType.ByUser)
                {
                    canBeActivated = true;
                }
            }
            foreach (var property in data.properties)
            {
                property.ApplyItemLevel(itemLevel);
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

        public void IncreaseCharge()
        {
            if (charge < data.requiredCharge)
            {
                charge++;
            }
        }


    }
}
