using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Items
{
    using ItemProperties;
    using ItemAbilities;
    using TextGameRPG.Scripts.Bot;
    using Localizations;

    public static class ItemViewBuilder
    {
        public static string Build(GameSession session, InventoryItem item)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<b>" + item.GetFullName(session) + "</b>");

            AppendGeneralItemInfo(sb, session, item);
            AppendPassiveHeader(sb, session, item);
            AppendAbilities(sb, session, item);
            AppendProperties(sb, session, item);

            AppendBottomInfo(sb, session, item);
            return sb.ToString();
        }

        private static void AppendGeneralItemInfo(StringBuilder sb, GameSession session, InventoryItem item)
        {
            var data = item.data;
            sb.Append(string.Format(Localization.Get(session, "item_view_general_info"), data.itemRarity.GetView(session), data.requiredLevel));

            
            if (item.data.ablitityByType.TryGetValue(AbilityType.DealDamage, out var dealDamage))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(dealDamage.GetView(session));
            }
            if (item.data.ablitityByType.TryGetValue(AbilityType.BlockIncomingDamageEveryTurn, out var blockDamage))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(blockDamage.GetView(session));
            }
            if (item.data.propertyByType.TryGetValue(PropertyType.DamageResist, out var damageResist))
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(damageResist.GetView(session));
            }
        }

        private static void AppendPassiveHeader(StringBuilder sb, GameSession session, InventoryItem item)
        {
            bool hasPassive = false;
            foreach (var ability in item.data.abilities)
            {
                switch (ability.abilityType)
                {
                    case AbilityType.DealDamage:
                    case AbilityType.BlockIncomingDamageEveryTurn:
                        continue;
                    default:
                        hasPassive = true;
                        break;
                }
            }
            if (!hasPassive)
            {
                foreach (var property in item.data.properties)
                {
                    switch (property.propertyType)
                    {
                        case PropertyType.DamageResist:
                            continue;
                        default:
                            hasPassive = true;
                            break;
                    }
                }
            }
            
            if (hasPassive)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(Localization.Get(session, "item_view_properties_header"));
            }
        }

        private static void AppendAbilities(StringBuilder sb, GameSession session, InventoryItem item)
        {
            foreach (var ability in item.data.abilities)
            {
                if (ability.abilityType != AbilityType.DealDamage && ability.abilityType != AbilityType.BlockIncomingDamageEveryTurn)
                {
                    sb.AppendLine();
                    sb.Append($"{Emojis.elements[Element.SmallBlack]} " + ability.GetView(session));
                }
            }
        }

        private static void AppendProperties(StringBuilder sb, GameSession session, InventoryItem item)
        {
            if (item.data.properties.Count > 0)
            {
                foreach (var property in item.data.properties)
                {
                    if (property.propertyType != PropertyType.DamageResist)
                    {
                        sb.AppendLine();
                        sb.Append($"{Emojis.elements[Element.SmallBlack]} " + property.GetView(session));
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
                sb.Append(string.Format(Localization.Get(session, "item_view_cost_of_use"), item.manaCost) + $" {Emojis.stats[Stat.Mana]}");
            }
            if (item.data.isChargeRequired)
            {
                sb.AppendLine();
                sb.AppendLine();
                sb.Append(string.Format(Localization.Get(session, "item_view_current_charge"), item.data.requiredCharge));
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
