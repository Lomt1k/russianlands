using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Resources;

public class BuyCrossroadsEnergyForDimondsDialog : DialogBase
{
    private static readonly ResourceData targetResource = new ResourceData(ResourceId.CrossroadsEnergy, 1);

    private readonly Func<Task> _onSuccess;
    private readonly Func<Task> _onCancel;

    public BuyCrossroadsEnergyForDimondsDialog(GameSession _session, Func<Task> onSuccess, Func<Task> onCancel) : base(_session)
    {
        _onSuccess = onSuccess;
        _onCancel = onCancel;
    }

    public override async Task Start()
    {
        var text = new StringBuilder()
            .AppendLine(Localization.Get(session, "resource_not_enough"))
            .AppendLine()
            .AppendLine(Localization.Get(session, "resource_header_resources"))
            .AppendLine(targetResource.GetLocalizedView(session))
            .AppendLine()
            .AppendLine(Localization.Get(session, "resource_purchase_for_diamonds"))
            .ToString();

        var secondsUntilEnergy = (int)ResourceHelper.RefreshCrossroadsEnergy(session).timeUntilNextEnergy.TotalSeconds;
        RegisterButton(GetActualPriceInDiamonds(secondsUntilEnergy).GetCompactView(), TryPurchase);
        RegisterBackButton(_onCancel);

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    private async Task TryPurchase()
    {
        var actualEnergy = ResourceHelper.RefreshCrossroadsEnergy(session);
        if (actualEnergy.resourceData.amount > 0)
        {
            var notification = Localization.Get(session, "resource_buying_energy_canceled");
            await notificationsManager.ShowNotification(session, notification, _onSuccess).FastAwait();
            return;
        }

        var actualPriceInDiamonds = GetActualPriceInDiamonds((int)actualEnergy.timeUntilNextEnergy.TotalSeconds);
        var playerResources = session.player.resources;
        var success = playerResources.TryPurchase(actualPriceInDiamonds);
        if (success)
        {
            Program.logger.Info($"INGAME BUY | Crossroads Energy: user {session.actualUser}");
            playerResources.ForceAdd(targetResource);
            session.profile.data.lastCrossroadsResourceUpdate = DateTime.UtcNow;

            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "resource_successfull_purshase_for_diamonds", actualPriceInDiamonds.GetCompactView()));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_resources"));
            sb.AppendLine(targetResource.GetLocalizedView(session));

            await notificationsManager.ShowNotification(session, sb, _onSuccess).FastAwait();
            return;
        }

        ClearButtons();
        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
        RegisterBackButton(_onCancel);

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    private ResourceData GetActualPriceInDiamonds(int secondsUntilEnergy)
    {
        return ResourceHelper.CalculateCrossroadsEnergyPriceInDiamonds(secondsUntilEnergy);
    }

}
