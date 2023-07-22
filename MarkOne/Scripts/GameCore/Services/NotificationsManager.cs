using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Dialogs.Town;
using MarkOne.Scripts.GameCore.Dialogs.Town.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.Payments;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Services;

public class NotificationsManager : Service
{
    private readonly PaymentManager paymentManager = ServiceLocator.Get<PaymentManager>();

    public async Task GetNotificationsAndEntryTown(GameSession session, TownEntryReason reason)
    {
        await CommonNotificationsLogic(session, () => new TownDialog(session, reason).Start()).FastAwait();
    }

    public async Task GetNotificationsAndOpenBuildingsDialog(GameSession session)
    {
        await CommonNotificationsLogic(session, () => new BuildingsDialog(session).Start()).FastAwait();
    }

    private async Task CommonNotificationsLogic(GameSession session, Func<Task> onNotificationsEnd)
    {
        // ingore notifications in tutorial
        if (session.tooltipController.hasTooltips)
        {
            await onNotificationsEnd().FastAwait();
            return;
        }

        if (NeedForceClaimResources(session))
        {
            await new TryCollectResourcesDialog(session, () => CommonNotificationsLogic(session, onNotificationsEnd)).Start().FastAwait();
            return;
        }

        var notification = new StringBuilder();
        var playerBuildings = session.player.buildings.GetAllBuildings();
        var buildingsData = session.profile.buildingsData;
        foreach (var building in playerBuildings)
        {
            if (!building.HasImportantUpdates(buildingsData))
                continue;

            var header = $"<pre>{building.GetLocalizedName(session, session.profile.buildingsData)}:</pre>";
            notification.AppendLine(header);

            var updates = building.GetUpdates(session, buildingsData, onlyImportant: true);
            foreach (var update in updates)
            {
                notification.AppendLine(Emojis.ElementSmallBlack + update);
            }
            notification.AppendLine();
        }

        if (notification.Length > 0)
        {
            await ShowNotification(session, notification, () => CommonNotificationsLogic(session, onNotificationsEnd)).FastAwait();
            return;
        }

        var specialNotification = session.profile.data.specialNotification;
        if (!string.IsNullOrEmpty(specialNotification))
        {
            notification
                .AppendLine(Localization.Get(session, "special_notification_header"))
                .AppendLine()
                .Append(specialNotification);
            session.profile.data.specialNotification = string.Empty;
            await ShowNotification(session, notification, () => CommonNotificationsLogic(session, onNotificationsEnd)).FastAwait();
            return;
        }

        var hasWaitingGoods = session.profile.data.hasWaitingGoods;
        if (hasWaitingGoods)
        {
            await paymentManager.GetNextWaitingGoods(session, () => CommonNotificationsLogic(session, onNotificationsEnd)).FastAwait();
            return;
        }

        if (session.player.IsPremiumActive())
        {
            var now = DateTime.UtcNow;
            if (session.profile.data.lastPremiumDailyRewardTime.Date != now.Date)
            {
                Program.logger.Info($"User {session.actualUser} get premium daily reward");
                var dailyRewards = ShopDialogPanel.premiumDailyRewards;
                session.profile.data.lastPremiumDailyRewardTime = now;
                session.player.resources.ForceAdd(dailyRewards);
                notification.AppendLine(Localization.Get(session, "resource_header_premium_daily_reward"));
                notification.AppendLine(dailyRewards.GetLocalizedView(session, showCountIfSingle: false));
                await ShowNotification(session, notification, () => CommonNotificationsLogic(session, onNotificationsEnd)).FastAwait();
                return;
            }
        }

        await onNotificationsEnd().FastAwait();
    }

    private bool NeedForceClaimResources(GameSession session)
    {
        var buildingsData = session.profile.buildingsData;
        var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
        foreach (ProductionBuildingBase building in productionBuildings)
        {
            if (!building.IsBuilt(buildingsData))
                continue;

            var farmedAmount = building.GetFarmedResourceAmount(buildingsData);
            var farmLimit = building.GetCurrentLevelResourceLimit(buildingsData);
            var isFarmedLimitReached = farmedAmount >= farmLimit;

            if (isFarmedLimitReached)
            {
                var isStorageLimitReached = session.player.resources.IsResourceLimitReached(building.resourceId);
                if (!isStorageLimitReached)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public async Task ShowNotification(GameSession session, StringBuilder text, Func<Task> onButtonClick)
    {
        var dialog = new SimpleDialog(session, text.ToString(), false, new Dictionary<string, Func<Task>>
        {
            { Localization.Get(session, "menu_item_continue_button"), onButtonClick }
        });
        await dialog.Start().FastAwait();
    }

    public async Task ShowNotification(GameSession session, string text, Func<Task> onButtonClick)
    {
        var dialog = new SimpleDialog(session, text, false, new Dictionary<string, Func<Task>>
        {
            { Localization.Get(session, "menu_item_continue_button"), onButtonClick }
        });
        await dialog.Start().FastAwait();
    }

}
