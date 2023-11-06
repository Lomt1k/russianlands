using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Dialogs.Events.DailyBonus;
using MarkOne.Scripts.GameCore.Dialogs.Events.ReferralSystem;
using MarkOne.Scripts.GameCore.Dialogs.Town;
using MarkOne.Scripts.GameCore.Dialogs.Town.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.Payments;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop.Offers;

namespace MarkOne.Scripts.GameCore.Services;

public class NotificationsManager : Service
{
    private readonly PaymentManager paymentManager = ServiceLocator.Get<PaymentManager>();
    private readonly OffersManager offersManager = ServiceLocator.Get<OffersManager>();

    public async Task GetNotificationsAndEntryTown(GameSession session, TownEntryReason reason)
    {
        await CommonNotificationsLogic(session, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
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

        // collect resources from buildings
        var playerBuildings = session.player.buildings.GetAllBuildings();
        var buildingsData = session.profile.buildingsData;
        foreach (var building in playerBuildings)
        {
            if (!building.HasImportantUpdates(buildingsData))
                continue;

            var header = building.GetLocalizedName(session, session.profile.buildingsData) + ':';
            notification.AppendLine(header.CodeBlock());

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

        await onNotificationsEnd().FastAwait();
    }

    private bool NeedForceClaimResources(GameSession session)
    {
        var buildingsData = session.profile.buildingsData;

        var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
        foreach (ProductionBuildingBase building in productionBuildings)
        {
            if (!building.IsBuilt(buildingsData))
            {
                continue;
            }

            building.UpdateProduction(buildingsData);
            var storageAmount = building.GetStorageResourceAmount(buildingsData);
            var farmLimit = building.GetCurrentLevelResourceLimit(buildingsData);
            var isFarmedLimitReached = storageAmount >= farmLimit;

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

    private async Task ShowTownNotificationsAndEntryTown(GameSession session, TownEntryReason reason)
    {
        // ingore notifications in tutorial
        if (session.tooltipController.hasTooltips)
        {
            await new TownDialog(session, reason).Start().FastAwait();
            return;
        }

        var notification = new StringBuilder();

        // special notification
        var specialNotification = session.profile.data.specialNotification;
        if (!string.IsNullOrEmpty(specialNotification))
        {
            notification
                .AppendLine(Localization.Get(session, "special_notification_header"))
                .AppendLine()
                .Append(specialNotification);
            session.profile.data.specialNotification = string.Empty;
            await ShowNotification(session, notification, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
            return;
        }

        // take goods from shop
        var hasWaitingGoods = session.profile.data.hasWaitingGoods;
        if (hasWaitingGoods)
        {
            await paymentManager.GetNextWaitingGoods(session, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
            return;
        }

        // daily reward
        if (DailyBonusDialog.IsNewRewardAvailable(session))
        {
            await DailyBonusDialog.ClaimNewRewardAndShowNotification(session, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
            return;
        }

        // referral system rewards
        if (ReferralSystemDialog.IsNewRewardAvailable(session))
        {
            await ReferralSystemDialog.ClaimNewRewardAndShowNotification(session, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
            return;
        }

        // premium daily reward
        if (session.player.IsPremiumActive())
        {
            var now = DateTime.UtcNow;
            if (session.profile.data.lastPremiumDailyRewardTime.Date != now.Date)
            {
                var dailyRewards = ShopDialogPanel.premiumDailyRewards;
                session.profile.data.lastPremiumDailyRewardTime = now;
                session.player.resources.ForceAdd(dailyRewards);
                notification.AppendLine(Localization.Get(session, "resource_header_premium_daily_reward"));
                notification.AppendLine(dailyRewards.GetLocalizedView(session, showCountIfSingle: false));
                await ShowNotification(session, notification, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
                return;
            }
        }

        // offers: start new offer
        var dtNow = DateTime.UtcNow;
        var isNewOfferStarted = false;
        if ((dtNow - session.lastStartOfferTime).TotalMinutes > 10)
        {
            var newOffer = await offersManager.TryStartNextOffer(session).FastAwait();
            if (newOffer is not null)
            {
                Program.logger.Info($"Started offer '{newOffer.GetData().GetTitle(session)}' (ID {newOffer.id}) for user {session.actualUser}");
                session.lastStartOfferTime = dtNow;
                isNewOfferStarted = true;
                await newOffer.GetData().StartOfferDialog(session, newOffer, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
                return;
            }
        }

        // offers: reminder
        if (!isNewOfferStarted && session.profile.data.lastOfferReminderTime.Date != dtNow.Date)
        {
            session.profile.data.lastOfferReminderTime = dtNow;
            var offer = session.profile.dynamicData.offers
                .Where(x => x.IsActive())
                .OrderBy(x => x.GetTimeToEnd())
                .FirstOrDefault();

            if (offer is not null)
            {
                await offer.GetData().StartOfferDialog(session, offer, () => ShowTownNotificationsAndEntryTown(session, reason)).FastAwait();
                return;
            }
        }

        await new TownDialog(session, reason).Start().FastAwait();
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

    public async Task ShowNotification(GameSession session, InputFile photo, StringBuilder text, Func<Task> onButtonClick)
    {
        var dialog = new SimpleDialog(session, photo, text.ToString(), false, new Dictionary<string, Func<Task>>
        {
            { Localization.Get(session, "menu_item_continue_button"), onButtonClick }
        });
        await dialog.Start().FastAwait();
    }

    public async Task ShowNotification(GameSession session, InputFile photo, string text, Func<Task> onButtonClick)
    {
        var dialog = new SimpleDialog(session, photo, text, false, new Dictionary<string, Func<Task>>
        {
            { Localization.Get(session, "menu_item_continue_button"), onButtonClick }
        });
        await dialog.Start().FastAwait();
    }

}
