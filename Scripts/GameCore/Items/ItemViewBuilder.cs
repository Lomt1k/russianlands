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

            AppendEquippedState(sb, session, item);
            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.AppendLine(string.Format(Localization.Get(session, "item_view_general_info"), data.itemRarity.GetView(session), data.requiredLevel));
        }

        private static void AppendDamageInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            if (!data.propertyByType.TryGetValue(ItemPropertyType.DealDamage, out var damageProperty))
                return;

            sb.AppendLine();
            sb.AppendLine(damageProperty.GetView(session));
        }

        private static void AppendDamageResistanceInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            if (!data.propertyByType.TryGetValue(ItemPropertyType.DamageResist, out var damageResistProperty))
                return;

            sb.AppendLine();
            sb.AppendLine(damageResistProperty.GetView(session));
        }

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

        private static void AppendEquippedState(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (item.isEquipped)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_inventory_equipped_state"));
            }
        }

    }
}
