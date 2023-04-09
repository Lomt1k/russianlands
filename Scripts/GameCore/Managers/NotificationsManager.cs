using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Dialogs;
using TextGameRPG.Scripts.Bot.Dialogs.Town;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.GameCore.Managers
{
    public class NotificationsManager : GlobalManager
    {
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
                    var isStorageLimitReached = session.player.resources.IsResourceLimitReached(building.resourceType);
                    if (!isStorageLimitReached)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task ShowNotification(GameSession session, StringBuilder text, Func<Task> onButtonClick)
        {
            var dialog = new SimpleDialog(session, text.ToString(), false, new Dictionary<string, Func<Task>>
            {
                { Localization.Get(session, "menu_item_continue_button"), onButtonClick }
            });
            await dialog.Start().FastAwait();
        }

    }
}
