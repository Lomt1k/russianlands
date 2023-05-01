using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public partial class PotionsDialogPanel : DialogPanelBase
    {
        private async Task ShowPotionInfo(PotionItem item)
        {
            ClearButtons();
            if (!item.IsReady())
            {
                var diamondsForBoost = item.GetBoostPriceInDiamonds();
                var priceView = ResourceId.Diamond.GetEmoji().ToString() + diamondsForBoost;
                var boostButtonText = Localization.Get(session, "menu_item_boost_button", priceView);
                RegisterButton(boostButtonText, () => TryBoostCraft(item));
                RegisterButton(Emojis.ElementCancel + Localization.Get(session, "dialog_potions_cancel_craft_button"),
                    () => CancelCraft(item));
            }

            RegisterBackButton(() => ShowPotionsList());

            var text = item.GetView(session);
            await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
        }

        private async Task TryBoostCraft(PotionItem item)
        {
            if (item.IsReady())
            {
                var message = Localization.Get(session, "dialog_potions_craft_boost_expired");
                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowPotionsList());
                await SendPanelMessage(message, GetOneLineKeyboard()).FastAwait();
                return;
            }

            var requiredDiamonds = item.GetBoostPriceInDiamonds();
            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceId.Diamond, requiredDiamonds, out var notEnoughDiamonds);
            if (successsPurchase)
            {
                item.BoostProduction();
                playerPotions.SortByPreparationTime();

                ClearButtons();
                var sb = new StringBuilder();
                sb.AppendLine(item.GetName(session).Bold());
                sb.AppendLine();
                sb.AppendLine(Emojis.ElementClock + Localization.Get(session, "dialog_potions_craft_boosted"));
                if (requiredDiamonds > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceId.Diamond.GetLocalizedView(session, requiredDiamonds));
                }

                RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowPotionsList());

                await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
                return;
            }

            ClearButtons();
            var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
                () => new ShopDialog(session).Start());
            RegisterBackButton(() => ShowPotionInfo(item));

            await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
        }

        private async Task CancelCraft(PotionItem item)
        {
            var alchemyLabLevel = item.GetData().workshopLevel;
            var alchemyLab = (AlchemyLabBuilding)BuildingId.AlchemyLab.GetBuilding();
            var resourcesToRestore = alchemyLab.GetCraftCostForBuildingLevel(alchemyLabLevel);
            session.player.resources.ForceAdd(resourcesToRestore);
            session.player.potions.Remove(item);

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_potions_craft_canceled"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_resources"));
            sb.Append(ResourceHelper.GetResourcesView(session, resourcesToRestore));

            RegisterBackButton(Localization.Get(session, "menu_item_continue_button"), () => ShowPotionsList());

            await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
        }


    }
}
