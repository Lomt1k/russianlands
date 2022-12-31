using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.General;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class PotionItemInfoDialog : DialogBase
    {
        private PotionItem _item;

        public PotionItemInfoDialog(GameSession session, PotionItem item) : base(session)
        {
            _item = item;

            if (!_item.IsReady())
            {
                var diamondsForBoost = _item.GetBoostPriceInDiamonds();
                var priceView = Emojis.resources[ResourceType.Diamond] + diamondsForBoost;
                var boostButtonText = string.Format(Localization.Get(session, "menu_item_boost_button"), priceView);
                RegisterButton(boostButtonText, () => TryBoostCraft());

                RegisterButton($"{Emojis.elements[Element.Cancel]} {Localization.Get(session, "dialog_potions_cancel_craft_button")}",
                    () => CancelCraft());
            }

            RegisterBackButton($"{Localization.Get(session, "menu_item_potions")} {Emojis.menuItems[MenuItem.Potions]}",
                () => new PotionsDialog(session).Start());
            RegisterDoubleBackButton($"{Localization.Get(session, "menu_item_character")} {Emojis.characters[CharIcon.Male]}",
                () => new TownCharacterDialog(session).Start());
        }

        public override async Task Start()
        {
            var text = _item.GetView(session);
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task TryBoostCraft()
        {
            if (_item.IsReady())
            {
                var message = Localization.Get(session, "dialog_potions_craft_boost_expired");
                ClearButtons();
                RegisterBackButton(() => new PotionsDialog(session).Start());
                await SendDialogMessage(message, GetOneLineKeyboard())
                    .ConfigureAwait(false);
            }

            var requiredDiamonds = _item.GetBoostPriceInDiamonds();
            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceType.Diamond, requiredDiamonds, out var notEnoughDiamonds);
            if (successsPurchase)
            {
                _item.BoostProduction();
                session.player.potions.SortByPreparationTime();

                ClearButtons();
                var sb = new StringBuilder();
                sb.AppendLine($"{Emojis.elements[Element.Clock]} {Localization.Get(session, "dialog_potions_craft_boosted")}");
                if (requiredDiamonds > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceType.Diamond.GetLocalizedView(session, requiredDiamonds));
                }

                RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                    () => new PotionsDialog(session).Start());

                await SendDialogMessage(sb, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            ClearButtons();
            var text = string.Format(Localization.Get(session, "resource_not_enough_diamonds"), Emojis.smiles[Smile.Sad]);
            RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}",
                () => new ShopDialog(session).Start());
            RegisterBackButton(() => new PotionItemInfoDialog(session, _item).Start());

            await SendDialogMessage(text, GetMultilineKeyboard()).ConfigureAwait(false);
        }

        private async Task CancelCraft()
        {
            var alchemyLabLevel = _item.GetData().workshopLevel;
            var alchemyLab = (AlchemyLabBuilding)BuildingType.AlchemyLab.GetBuilding();
            var resourcesToRestore = alchemyLab.GetCraftCostForBuildingLevel(alchemyLabLevel);
            session.player.resources.ForceAdd(resourcesToRestore);
            session.player.potions.Remove(_item);

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_potions_craft_canceled"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_resources"));
            sb.Append(ResourceHelper.GetResourcesView(session, resourcesToRestore));

            RegisterBackButton($"{Localization.Get(session, "menu_item_potions")} {Emojis.menuItems[MenuItem.Potions]}",
                () => new PotionsDialog(session).Start());

            await SendDialogMessage(sb, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
