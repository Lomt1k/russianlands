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
            AppendAbilities(sb, session, item);
            AppendProperties(sb, session, item);

            AppendEquippedState(sb, session, item);
            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.AppendLine(string.Format(Localization.Get(session, "item_view_general_info"), data.itemRarity.GetView(session), data.requiredLevel));
        }

        private static void AppendAbilities(StringBuilder sb, GameSession session, InventoryItem item)
        {
            foreach (var ability in item.data.abilities)
            {
                sb.AppendLine();
                sb.AppendLine(ability.GetView(session));
            }
        }

        private static void AppendProperties(StringBuilder sb, GameSession session, InventoryItem item)
        {
            foreach (var property in item.data.properties)
            {
                sb.AppendLine();
                sb.AppendLine(property.GetView(session));
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
