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
            var data = item.data;
            if (!data.HasDamageProperties())
                return;

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_deals_damage"));

            data.propertyByType.TryGetValue(ItemPropertyType.PhysicalDamage, out var physicalDamage);
            if (physicalDamage != null)
            {
                var property = (PhysicalDamageItemProperty)physicalDamage;
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.FireDamage, out var fireDamage);
            if (fireDamage != null)
            {
                var property = (FireDamageItemProperty)fireDamage;
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.FireDamage]} {value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.ColdDamage, out var coidDamage);
            if (coidDamage != null)
            {
                var property = (ColdDamageItemProperty)coidDamage;
                string value = property.minDamage == property.maxDamage
                    ? property.maxDamage.ToString()
                    : $"{property.minDamage} - {property.maxDamage}";
                sb.AppendLine($"{Emojis.stats[Stat.ColdDamage]} {value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.LightningDamage, out var lightningDamage);
            if (lightningDamage != null)
            {
                var property = (LightningDamageItemProperty)lightningDamage;
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
            var data = item.data;
            if (!data.HasDamageResistProperties())
                return;

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_protects_from_damage"));

            data.propertyByType.TryGetValue(ItemPropertyType.PhysicalDamageResist, out var physicalDamage);
            if (physicalDamage != null)
            {
                var property = (PhysicalDamageResistItemProperty)physicalDamage;
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {property.value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.FireDamageResist, out var fireDamage);
            if (fireDamage != null)
            {
                var property = (FireDamageResistItemProperty)fireDamage;
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {property.value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.ColdDamageResist, out var coidDamage);
            if (coidDamage != null)
            {
                var property = (ColdDamageResistItemProperty)coidDamage;
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {property.value}");
            }

            data.propertyByType.TryGetValue(ItemPropertyType.LightningDamageResist, out var lightningDamage);
            if (lightningDamage != null)
            {
                var property = (LightningDamageResistItemProperty)lightningDamage;
                sb.AppendLine($"{Emojis.stats[Stat.PhysicalDamage]} {property.value}");
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
