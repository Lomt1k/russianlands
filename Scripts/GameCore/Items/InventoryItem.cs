using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using TextGameRPG.Scripts.GameCore.Items.Generators;
    using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;

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

        public string GetFullName(GameSession session)
        {
            var sb = new StringBuilder();
            sb.Append($"{Emojis.items[data.itemType]} {GetLocalizationName(session)}");
            if (mod > 0)
            {
                sb.Append($" +{mod}");
            }
            var statIcons = data.statIcons;
            if (statIcons.Count > 0)
            {
                sb.Append(' ');
                foreach (var stat in statIcons)
                {
                    sb.Append(Emojis.stats[stat]);
                }
            }

            return sb.ToString();
        }

        private string GetLocalizationName(GameSession session)
        {
            if (!data.isGeneratedItem)
            {
                return Localizations.Localization.Get(session, $"item_name_{id}");
            }

            if (data.itemType == ItemType.Scroll)
            {
                var grade = data.grade;
                var hall = data.requiredTownHall;
                var prefix = Localizations.Localization.Get(session, $"item_scroll_prefix_hall_{hall}_grade_{grade}");
                var scroll = Localizations.Localization.Get(session, "item_scroll");

                var suffix = string.Empty;
                if (data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var ability))
                {
                    var dealDamage = (DealDamageAbility)ability;
                    var damageType = dealDamage.GetDamageTypeForScroll().ToString().ToLower();
                    suffix = Localizations.Localization.Get(session, $"item_scroll_name_suffix_{damageType}_mana_{manaCost}");
                }
                return prefix + ' ' + scroll + ' ' + suffix;
            }

            var itemType = data.itemType.ToString().ToLower();
            return Localizations.Localization.Get(session, $"item_{itemType}_hall_{data.requiredTownHall}_grade_{data.grade}");
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
