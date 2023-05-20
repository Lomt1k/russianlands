using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Character.Potions;

public partial class PotionsDialogPanel : DialogPanelBase
{
    public List<PotionItem> playerPotions => session.player.potions;

    public PotionsDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowPotionsList().FastAwait();
    }

    private async Task ShowPotionsList()
    {
        var readyPotionsCount = playerPotions.GetReadyPotionsCount();
        var inProductionCount = playerPotions.Count - readyPotionsCount;

        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_potions_ready_amount", readyPotionsCount));
        sb.AppendLine(Localization.Get(session, "dialog_potions_in_production_amount", inProductionCount));

        ClearButtons();
        foreach (var potionItem in playerPotions)
        {
            RegisterButton(potionItem.GetNameForList(session), () => ShowPotionInfo(potionItem));
        }

        if (playerPotions.HasPotionsInProduction())
        {
            var diamondsForBoost = new ResourceData(ResourceId.Diamond, 0);
            foreach (var potion in playerPotions.GetPotionsInProduction())
            {
                diamondsForBoost.amount += potion.GetBoostPriceInDiamonds().amount;
            }
            var priceView = diamondsForBoost.GetCompactView(shortView: false);
            var boostButtonText = Localization.Get(session, "menu_item_boost_all_button", priceView);
            RegisterButton(boostButtonText, TryBoostAllCraft);
        }

        var freeSlots = playerPotions.GetFreeSlotsCount(session);
        RegisterButton(Emojis.ElementPlus + Localization.Get(session, "dialog_potions_produce_button") + $" ({freeSlots})", TryOpenProductionPanel);

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task TryOpenProductionPanel()
    {
        var isFull = session.player.potions.IsFull(session);
        if (isFull)
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_potions_limit"));

            if (!session.profile.data.IsPremiumActive())
            {
                sb.AppendLine();
                sb.AppendLine(Emojis.StatPremium + Localization.Get(session, "menu_item_premium").Bold());
                sb.AppendLine(Localization.Get(session, "dialog_potions_limit_can_buy_premium"));
                RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
            }

            RegisterBackButton(ShowPotionsList);
            await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
            return;
        }

        await ShowPotionsToProductionList().FastAwait();
    }

    private async Task TryBoostAllCraft()
    {
        if (!playerPotions.HasPotionsInProduction())
        {
            var message = Localization.Get(session, "dialog_potions_craft_boost_expired");
            ClearButtons();
            RegisterBackButton(ShowPotionsList);
            await SendPanelMessage(message, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var requiredDiamonds = new ResourceData(ResourceId.Diamond, 0);
        foreach (var potion in playerPotions.GetPotionsInProduction())
        {
            requiredDiamonds.amount += potion.GetBoostPriceInDiamonds().amount;
        }

        var playerResources = session.player.resources;
        var successsPurchase = playerResources.TryPurchase(requiredDiamonds, out var notEnoughDiamonds);
        if (successsPurchase)
        {
            foreach (var potion in playerPotions.GetPotionsInProduction())
            {
                potion.BoostProduction();
            }

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ElementClock + Localization.Get(session, "dialog_potions_craft_boosted"));
            if (requiredDiamonds.amount > 0)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                sb.AppendLine(requiredDiamonds.GetLocalizedView(session));
            }

            RegisterButton(Localization.Get(session, "menu_item_continue_button"), ShowPotionsList);

            await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
            return;
        }

        ClearButtons();
        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
            () => new ShopDialog(session).Start());
        RegisterBackButton(ShowPotionsList);

        await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
    }

}
