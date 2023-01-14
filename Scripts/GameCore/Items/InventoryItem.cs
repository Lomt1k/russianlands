using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using System.Collections.Generic;
    using TextGameRPG.Scripts.GameCore.Items.Generators;
    using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
    using TextGameRPG.Scripts.GameCore.Resources;

    public enum ItemState : byte { IsNewAndNotEquipped = 0, IsNotEquipped = 1, IsEquipped = 2 }

    public class InventoryItem
    {
        public static byte requiredStickCharge = 3;

        [JsonProperty]
        public string id;
        [JsonProperty("s")]
        public ItemState state;

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonIgnore]
        public sbyte manaCost { get; private set; }

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

        public InventoryItem(int _id)
        {
            id = _id.ToString();
            RecalculateDynamicData();
        }

        public InventoryItem(string _dataCode)
        {
            id = _dataCode;
            RecalculateDynamicData();
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem()
            {
                id = id,
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
                manaCost += ability.manaCost;
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
            sb.Append(data.itemType.GetEmoji() + GetLocalizationName(session));

            var statIcons = data.statIcons;
            if (statIcons.Count > 0)
            {
                sb.Append(' ');
                foreach (var stat in statIcons)
                {
                    sb.Append(stat.GetEmoji());
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

        public Dictionary<ResourceType,int> CalculateResourcesForBreakApart()
        {
            var result = new Dictionary<ResourceType,int>();
            var resourceType = data.itemRarity switch
            {
                Rarity.Common => ResourceType.CraftPiecesCommon,
                Rarity.Rare => ResourceType.CraftPiecesRare,
                Rarity.Epic => ResourceType.CraftPiecesEpic,
                _ => ResourceType.CraftPiecesLegendary
            };
            result.Add(resourceType, 1);
            return result;
        }


    }
}
