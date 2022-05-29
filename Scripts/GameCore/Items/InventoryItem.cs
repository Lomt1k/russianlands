using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    public struct DynamicDamageProperty
    {
        public int minDamage;
        public int maxDamage;
    }

    public struct DynamicDamageResistProperty
    {
        public int value;
    }

    public class InventoryItem
    {
        public int itemId { get; set; }
        public int itemLevel { get; set; }
        public bool isEquipped { get; set; }
        public bool isNew { get; set; } = true;

        [JsonIgnore]
        public ItemData data { get; private set; }

        [JsonIgnore]
        public Dictionary<ItemPropertyType, DynamicDamageProperty> dynamicDamageProperties = new Dictionary<ItemPropertyType, DynamicDamageProperty>();

        [JsonIgnore]
        public Dictionary<ItemPropertyType, DynamicDamageResistProperty> dynamicDamageResistProperties = new Dictionary<ItemPropertyType, DynamicDamageResistProperty>();

        [JsonConstructor]
        private InventoryItem()
        {
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            data = GameDataBase.GameDataBase.instance.items[itemId];
            RecalculateDynamicProperties();
        }

        public InventoryItem(ItemData data)
        {
            this.data = data;
            itemId = data.id;
            RecalculateDynamicProperties();
        }

        public InventoryItem(int id)
        {
            itemId = id;
            data = GameDataBase.GameDataBase.instance.items[itemId];
            RecalculateDynamicProperties();
        }

        public InventoryItem Clone()
        {
            var clone = new InventoryItem(data);
            clone.itemLevel = itemLevel;
            clone.isEquipped = false;
            clone.isNew = true;
            RecalculateDynamicProperties();
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

        private void RecalculateDynamicProperties()
        {
            RecalculateDynamicDamageProperties();
            RecalculateDynamicDamageResistProperties();
        }

        private void RecalculateDynamicDamageProperties()
        {
            dynamicDamageProperties.Clear();
            foreach (var property in data.GetDamageProperties())
            {
                int minDamage = 0;
                int maxDamage = 0;
                
                switch (property)
                {
                    case PhysicalDamageItemProperty physicalDamage:
                        minDamage = physicalDamage.minDamage;
                        maxDamage = physicalDamage.maxDamage;
                        break;
                    case ColdDamageItemProperty coldDamage:
                        minDamage = coldDamage.minDamage;
                        maxDamage = coldDamage.maxDamage;
                        break;
                    case FireDamageItemProperty fireDamage:
                        minDamage = fireDamage.minDamage;
                        maxDamage = fireDamage.maxDamage;
                        break;
                    case LightningDamageItemProperty lightningDamage:
                        minDamage = lightningDamage.minDamage;
                        maxDamage = lightningDamage.maxDamage;
                        break;
                }

                float minDamageBonusPerLevel = minDamage / 10 > 0 ? (float)minDamage / 10 : 1;
                float maxDamageBonusPerLevel = maxDamage / 10 > 0 ? (float)maxDamage / 10 : 1;
                var dynamicProperty = new DynamicDamageProperty
                {
                    minDamage = minDamage + (int)(minDamageBonusPerLevel * itemLevel),
                    maxDamage = maxDamage + (int)(maxDamageBonusPerLevel * itemLevel),
                };
                dynamicDamageProperties.Add(property.propertyType, dynamicProperty);
            }
        }

        private void RecalculateDynamicDamageResistProperties()
        {
            dynamicDamageResistProperties.Clear();
            foreach (var property in data.GetDamageResistProperties())
            {
                int damageResist = 0;

                switch (property)
                {
                    case PhysicalDamageResistItemProperty physicalDamage:
                        damageResist = physicalDamage.value;
                        break;
                    case ColdDamageResistItemProperty coldDamage:
                        damageResist = coldDamage.value;
                        break;
                    case FireDamageResistItemProperty fireDamage:
                        damageResist = fireDamage.value;
                        break;
                    case LightningDamageResistItemProperty lightningDamage:
                        damageResist = lightningDamage.value;
                        break;
                }

                float resistBonusPerLevel = damageResist / 10 > 0 ? (float)damageResist / 10 : 1;
                var dynamicProperty = new DynamicDamageResistProperty
                {
                    value = damageResist + (int)(resistBonusPerLevel * itemLevel)
                };
                dynamicDamageResistProperties.Add(property.propertyType, dynamicProperty);
            }
        }

    }
}
