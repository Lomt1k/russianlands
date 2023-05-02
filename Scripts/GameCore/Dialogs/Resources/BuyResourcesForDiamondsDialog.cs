using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Resources;

public class BuyResourcesForDiamondsDialog : DialogBase
{
    private readonly IEnumerable<ResourceData> _targetResources;
    private readonly int _priceInDiamonds;
    private readonly Func<Task> _onSuccess;
    private readonly Func<Task> _onCancel;

    public BuyResourcesForDiamondsDialog(GameSession _session, IEnumerable<ResourceData> targetResources, Func<Task> onSuccess, Func<Task> onCancel) : base(_session)
    {
        _targetResources = targetResources;
        _onSuccess = onSuccess;
        _onCancel = onCancel;

        foreach (var resourceData in _targetResources)
        {
            _priceInDiamonds += ResourceHelper.CalculatePriceInDiamonds(resourceData);
        }
    }

    public BuyResourcesForDiamondsDialog(GameSession _session, ResourceData targetResource, Func<Task> onSuccess, Func<Task> onCancel)
        : this(_session, new ResourceData[] { targetResource }, onSuccess, onCancel) { }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "resource_not_enough"));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_header_resources"));
        foreach (var resourceData in _targetResources)
        {
            sb.AppendLine(resourceData.GetLocalizedView(session));
        }

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_purchase_for_diamonds"));
        RegisterButton(ResourceId.Diamond.GetEmoji() + _priceInDiamonds.ToString(), TryPurchase);
        RegisterBackButton(_onCancel);

        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task TryPurchase()
    {
        var playerResources = session.player.resources;
        var success = playerResources.TryPurchase(new ResourceData(ResourceId.Diamond, _priceInDiamonds));
        if (success)
        {
            playerResources.ForceAdd(_targetResources);
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "resource_successfull_purshase_for_diamonds"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_resources"));
            foreach (var resourceData in _targetResources)
            {
                sb.AppendLine(resourceData.GetLocalizedView(session));
            }

            await messageSender.SendTextMessage(session.chatId, sb.ToString()).FastAwait();
            await _onSuccess().FastAwait();
            return;
        }

        ClearButtons();
        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
            () => new ShopDialog(session).Start());
        RegisterBackButton(_onCancel);

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

}
