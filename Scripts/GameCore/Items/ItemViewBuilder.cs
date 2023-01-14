using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using ItemAbilities;
    using TextGameRPG.Scripts.Bot;
    using Localizations;

    // влияет на порядок отрисовки абилки в описании предмета
    public enum ViewPriority : byte
    {
        GeneralInfo = 0,
        SecondoryInfo = 1,
        Passive = 2,
    }

    public static class ItemViewBuilder
    {
        public static string Build(GameSession session, InventoryItem item)
        {
            var sb = new StringBuilder();
            sb.AppendLine(item.GetFullName(session).Bold());

            AppendGeneralItemInfo(sb, session, item);
            AppendPassiveBonuses(sb, session, item);
            AppendBottomInfo(sb, session, item);
            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.Append(Localization.Get(session, "item_view_general_info", data.itemRarity.GetView(session), data.requiredLevel));

            // general info
            foreach (var ability in item.data.abilities)
            {
                if (ability.abilityType.GetPriority() == ViewPriority.GeneralInfo)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.Append(ability.GetView(session));
                }
            }
            foreach (var property in item.data.properties)
            {
                if (property.propertyType.GetPriority() == ViewPriority.GeneralInfo)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.Append(property.GetView(session));
                }
            }

            //secondary info
            foreach (var ability in item.data.abilities)
            {
                if (ability.abilityType.GetPriority() == ViewPriority.SecondoryInfo)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.Append(ability.GetView(session));
                }
            }
        }

        private static void AppendPassiveBonuses(StringBuilder sb, GameSession session, InventoryItem item)
        {
            bool hasPassiveAbilities = false;
            bool hasPassiveProperties = false;
            foreach (var ability in item.data.abilities)
            {
                if (ability.abilityType.GetPriority() == ViewPriority.Passive)
                {
                    hasPassiveAbilities = true;
                    break;
                }
            }
            foreach (var property in item.data.properties)
            {
                if (property.propertyType.GetPriority() == ViewPriority.Passive)
                {
                    hasPassiveProperties = true;
                    break;
                }
            }
            if (!hasPassiveAbilities && !hasPassiveProperties)
                return;

            // passive header
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(Localization.Get(session, "item_view_properties_header"));

            // passive info
            if (hasPassiveAbilities)
            {
                foreach (var ability in item.data.abilities)
                {
                    if (ability.abilityType.GetPriority() == ViewPriority.Passive)
                    {
                        sb.AppendLine();
                        sb.Append(Emojis.ElementSmallBlack + ability.GetView(session));
                    }
                }
            }
            if (hasPassiveProperties)
            {
                foreach (var property in item.data.properties)
                {
                    if (property.propertyType.GetPriority() == ViewPriority.Passive)
                    {
                        sb.AppendLine();
                        sb.Append(Emojis.ElementSmallBlack + property.GetView(session));
                    }
                }
            }
        }

        private static void AppendBottomInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (item.manaCost > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Localization.Get(session, "item_view_cost_of_use", item.manaCost) + Emojis.StatMana);
            }
            if (item.data.itemType == ItemType.Stick)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Localization.Get(session, "item_view_current_charge", InventoryItem.requiredStickCharge));
            }
            if (item.isEquipped)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Localization.Get(session, "dialog_inventory_equipped_state"));
            }
        }

    }
}
