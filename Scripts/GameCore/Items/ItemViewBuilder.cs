using System.Text;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using ItemAbilities;
    using TextGameRPG.Scripts.TelegramBot;
    using Localizations;

    internal static class ItemViewBuilder
    {
        public static string Build(GameSession session, InventoryItem item)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>" + item.GetFullName(session) + "</b>");

            AppendGeneralItemInfo(sb, session, item);
            AppendAbilities(sb, session, item);
            AppendProperties(sb, session, item);

            AppendBottomInfo(sb, session, item);
            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.AppendLine(string.Format(Localization.Get(session, "item_view_general_info"), data.itemRarity.GetView(session), data.requiredLevel));
            sb.AppendLine();
        }

        private static void AppendAbilities(StringBuilder sb, GameSession session, InventoryItem item)
        {
            bool hasDealDamage = item.data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var dealDamage);
            if (hasDealDamage)
            {
                sb.AppendLine(dealDamage.GetView(session));
            }
            bool hasBlockDamage = item.data.ablitityByType.TryGetValue(AbilityType.BlockIncomingDamageEveryTurn, out var blockDamage);
            if (hasBlockDamage)
            {
                sb.AppendLine(blockDamage.GetView(session));
            }

            bool needAppendLine = false;
            foreach (var ability in item.data.abilities)
            {
                if (ability.abilityType != AbilityType.DealDamage 
                    && ability.abilityType != AbilityType.BlockIncomingDamageEveryTurn)
                {
                    sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} " + ability.GetView(session));
                    needAppendLine = true;
                }
            }

            if (needAppendLine)
                sb.AppendLine();
        }

        private static void AppendProperties(StringBuilder sb, GameSession session, InventoryItem item)
        {
            bool hasDamageResist = item.data.propertyByType.TryGetValue(PropertyType.DamageResist, out var damageResist);
            if (hasDamageResist)
            {
                sb.AppendLine(damageResist.GetView(session));
            }

            foreach (var property in item.data.properties)
            {
                if (property.propertyType != PropertyType.DamageResist)
                {
                    sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} " + property.GetView(session));
                }
            }
        }

        private static void AppendBottomInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (item.manaCost > 0)
            {
                sb.AppendLine(string.Format(Localization.Get(session, "item_view_cost_of_use"), item.manaCost)
                    + $" {Emojis.stats[Stat.Mana]}");
            }
            if (item.data.isChargeRequired)
            {
                sb.AppendLine(string.Format(Localization.Get(session, "item_view_current_charge"), item.charge, item.data.requiredCharge));
            }
            if (item.isEquipped)
            {           
                sb.AppendLine(Localization.Get(session, "dialog_inventory_equipped_state"));
            }
        }

    }
}
