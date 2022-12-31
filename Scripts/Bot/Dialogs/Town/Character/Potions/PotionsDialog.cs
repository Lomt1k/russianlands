using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Potions
{
    public class PotionsDialog : DialogBase
    {
        private PotionsDialogPanel _potionsPanel;

        public PotionsDialog(GameSession session) : base(session)
        {
            _potionsPanel = new PotionsDialogPanel(this, 0);
            RegisterPanel(_potionsPanel);

            var playerPotions = session.player.potions;
            var freeSlots = playerPotions.GetFreeSlotsCount(session);
            RegisterButton($"{Emojis.elements[Element.Plus]} {Localization.Get(session, "dialog_potions_produce_button")} ({freeSlots})",
                () => TryOpenProductionDialog());

            if (playerPotions.HasPotionsInProduction())
            {
                var diamondsForBoost = 0;
                foreach (var potion in playerPotions.GetPotionsInProduction())
                {
                    diamondsForBoost += potion.GetBoostPriceInDiamonds();
                }
                var priceView = Emojis.resources[ResourceType.Diamond] + diamondsForBoost;
                var boostButtonText = string.Format(Localization.Get(session, "menu_item_boost_button"), priceView);
                RegisterButton(boostButtonText, () => TryBoostAllCraft());
            }

            RegisterBackButton($"{Localization.Get(session, "menu_item_character")} {Emojis.characters[CharIcon.Male]}",
                () => new TownCharacterDialog(session).Start());
            RegisterTownButton(isDoubleBack: true);
        }

        public override async Task Start()
        {
            var amount = session.player.potions.Count;
            var header = $"<b>{Emojis.menuItems[MenuItem.Potions]} {Localization.Get(session, "menu_item_potions")} ({amount})</b>";
            await SendDialogMessage(header, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
            await SendPanelsAsync().ConfigureAwait(false);
        }

        private async Task TryOpenProductionDialog()
        {
            bool isFull = session.player.potions.IsFull(session);
            if (isFull)
            {
                ClearButtons();
                var sb = new StringBuilder();
                sb.AppendLine(Localization.Get(session, "dialog_potions_limit"));

                if (!session.profile.data.IsPremiumActive())
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "dialog_potions_limit_can_buy_premium"));
                    RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}", () => new ShopDialog(session).Start());
                }

                RegisterBackButton(() => new PotionsDialog(session).Start());
                await SendDialogMessage(sb, GetMultilineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            await new PotionsProductionDialog(session).Start()
                .ConfigureAwait(false);
        }

        private async Task TryBoostAllCraft()
        {
            var playerPotions = session.player.potions;
            if (!playerPotions.HasPotionsInProduction())
            {
                var message = Localization.Get(session, "dialog_potions_craft_boost_expired");
                ClearButtons();
                RegisterBackButton(() => new PotionsDialog(session).Start());
                await SendDialogMessage(message, GetOneLineKeyboard())
                    .ConfigureAwait(false);
            }

            var requiredDiamonds = 0;
            foreach (var potion in playerPotions.GetPotionsInProduction())
            {
                requiredDiamonds += potion.GetBoostPriceInDiamonds();
            }

            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceType.Diamond, requiredDiamonds, out var notEnoughDiamonds);
            if (successsPurchase)
            {
                foreach (var potion in playerPotions.GetPotionsInProduction())
                {
                    potion.BoostProduction();
                }

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
            RegisterBackButton(() => new PotionsDialog(session).Start());

            await SendDialogMessage(text, GetMultilineKeyboard()).ConfigureAwait(false);
        }

    }
}
