using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.Bot.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.General;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Resources;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Character.Potions;

public partial class PotionsDialogPanel : DialogPanelBase
{
    private async Task ShowPotionsToProductionList()
    {
        ClearButtons();
        var header = Localization.Get(session, "dialog_potions_produce_button").Bold();
        var alchemyLab = (AlchemyLabBuilding)BuildingId.AlchemyLab.GetBuilding();
        var potionsList = alchemyLab.GetPotionsForCurrentLevel(session.profile.buildingsData);
        foreach (var potionData in potionsList)
        {
            RegisterButton(potionData.GetName(session), () => ShowPotionsProductionAmountSelection(potionData));
        }
        RegisterBackButton(ShowPotionsList);

        await SendPanelMessage(header, GetMultilineKeyboard()).FastAwait();
    }

    public async Task ShowPotionsProductionAmountSelection(PotionData data)
    {
        ClearButtons();
        var countLimit = Math.Min(GetFreeSlotsCount(), 5);
        for (var i = 1; i <= countLimit; i++)
        {
            var amountForDelegate = i; //it is important!
            RegisterButton(i.ToString(), () => TryCraft(data, amountForDelegate));
        }
        RegisterBackButton(() => ShowPotionsToProductionList());

        var sb = new StringBuilder();
        sb.AppendLine(data.GetName(session).Bold());

        sb.AppendLine();
        sb.AppendLine(data.GetDescription(session, session));

        sb.AppendLine();
        var requiredResources = GetCraftCost();
        sb.Append(requiredResources.GetPriceView(session));
        var dtNow = DateTime.UtcNow;
        var timeSpan = dtNow.AddSeconds(GetCraftTimeInSeconds()) - dtNow;
        sb.AppendLine(timeSpan.GetView(session, withCaption: true));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_header_ours"));
        var ourResources = new ResourceData(ResourceId.Herbs, session.player.resources.GetValue(ResourceId.Herbs));
        sb.Append(ourResources.GetLocalizedView(session));

        sb.AppendLine();
        sb.Append(Localization.Get(session, "dialog_potions_select_potions_amount"));

        await SendPanelMessage(sb, GetSpecialKeyboard()).FastAwait();
    }

    private int GetFreeSlotsCount()
    {
        return playerPotions.GetFreeSlotsCount(session);
    }

    private ResourceData GetCraftCost()
    {
        var alchemyLab = (AlchemyLabBuilding)BuildingId.AlchemyLab.GetBuilding();
        return alchemyLab.GetCurrentCraftCost(session.profile.buildingsData);
    }

    private int GetCraftTimeInSeconds()
    {
        var alchemyLab = (AlchemyLabBuilding)BuildingId.AlchemyLab.GetBuilding();
        return alchemyLab.GetCurrentCraftTimeInSeconds(session.profile.buildingsData);
    }

    private InlineKeyboardMarkup GetSpecialKeyboard()
    {
        return GetKeyboardWithRowSizes(buttonsCount - 1, 1);
    }

    public async Task TryCraft(PotionData data, int potionsAmount)
    {
        var requiredResources = GetCraftCost();
        requiredResources.amount *= potionsAmount;

        var playerResources = session.player.resources;
        var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
        if (successfullPurchase)
        {
            StartCraft(data, potionsAmount);
            await ShowPotionsList().FastAwait();
            return;
        }

        var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
            onSuccess: async () => await new PotionsDialog(session).StartWithTryCraft(data, potionsAmount).FastAwait(),
            onCancel: async () => await new PotionsDialog(session).StartWithSelectionAmountToCraft(data).FastAwait());
        await buyResourcesDialog.Start().FastAwait();
    }

    private void StartCraft(PotionData data, int amount)
    {
        var craftEndTime = DateTime.UtcNow.AddSeconds(GetCraftTimeInSeconds()).Ticks;
        for (var i = 0; i < amount; i++)
        {
            var potionItem = new PotionItem(data.id, craftEndTime);
            session.player.potions.Add(potionItem);
        }
    }

}
