using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using System.Linq;
    using TextGameRPG.Scripts.TelegramBot;

    internal static class ItemViewBuilder
    {
        public static string Build(GameSession session, InventoryItem item)
        {
            var data = item.data;
            var sb = new StringBuilder();
            sb.AppendLine(item.GetFullName(session));
            sb.AppendLine();

            AppendDamageInfo(sb, session, item);

            return sb.ToString();
        }

        private static void AppendDamageInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            if (!data.HasDamageProperties())
                return;

            sb.AppendLine($"<b>Наносит урон:</b>");

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

    }
}
