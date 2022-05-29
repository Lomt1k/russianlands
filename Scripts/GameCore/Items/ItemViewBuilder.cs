using System.Text;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using TextGameRPG.Scripts.TelegramBot;
    using Localization;

    internal static class ItemViewBuilder
    {
        public static string Build(GameSession session, InventoryItem item)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>" + item.GetFullName(session) + "</b>");

            AppendGeneralItemInfo(sb, session, item);
            AppendDamageInfo(sb, session, item);
            AppendDamageResistanceInfo(sb, session, item);
            AppendSpecificProperties(sb, session, item);

            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.AppendLine(string.Format(Localization.Get(session, "item_view_general_info"), data.itemRarity.GetView(session), data.requiredLevel));
        }

        #region DamageInfo
        private static void AppendDamageInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (!item.data.HasDamageProperties())
                return;

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_deals_damage"));

            if (item.dynamicDamageProperties.ContainsKey(ItemPropertyType.PhysicalDamage))
            {
                var property = item.dynamicDamageProperties[ItemPropertyType.PhysicalDamage];
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {value}");
            }

            if (item.dynamicDamageProperties.ContainsKey(ItemPropertyType.FireDamage))
            {
                var property = item.dynamicDamageProperties[ItemPropertyType.FireDamage];
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.FireDamage]} {value}");
            }

            if (item.dynamicDamageProperties.ContainsKey(ItemPropertyType.ColdDamage))
            {
                var property = item.dynamicDamageProperties[ItemPropertyType.ColdDamage];
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.ColdDamage]} {value}");
            }

            if (item.dynamicDamageProperties.ContainsKey(ItemPropertyType.LightningDamage))
            {
                var property = item.dynamicDamageProperties[ItemPropertyType.LightningDamage];
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.LightningDamage]} {value}");
            }
        }
        #endregion

        #region DamageResistanceInfo
        private static void AppendDamageResistanceInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (!item.data.HasDamageResistProperties())
                return;

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_protects_from_damage"));

            if (item.dynamicDamageResistProperties.ContainsKey(ItemPropertyType.PhysicalDamageResist))
            {
                var property = item.dynamicDamageResistProperties[ItemPropertyType.PhysicalDamageResist];
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {property.value}");
            }

            if (item.dynamicDamageResistProperties.ContainsKey(ItemPropertyType.FireDamageResist))
            {
                var property = item.dynamicDamageResistProperties[ItemPropertyType.FireDamageResist];
                sb.AppendLine($"{Emojis.stats[Stat.FireDamage]} {property.value}");
            }

            if (item.dynamicDamageResistProperties.ContainsKey(ItemPropertyType.ColdDamageResist))
            {
                var property = item.dynamicDamageResistProperties[ItemPropertyType.ColdDamageResist];
                sb.AppendLine($"{Emojis.stats[Stat.ColdDamage]} {property.value}");
            }

            if (item.dynamicDamageResistProperties.ContainsKey(ItemPropertyType.LightningDamageResist))
            {
                var property = item.dynamicDamageResistProperties[ItemPropertyType.LightningDamageResist];
                sb.AppendLine($"{Emojis.stats[Stat.LightningDamage]} {property.value}");
            }
        }
        #endregion

        private static void AppendSpecificProperties(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            if (!data.HasSpecificProperties())
                return;

            sb.AppendLine();
            var specificProperties = item.data.GetSpecificProperties();
            foreach (var property in specificProperties)
            {
                sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} " + property.GetView(session));
            }
        }

    }
}
