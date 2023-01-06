using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
{
    public class TryCollectResourcesDialog : DialogBase
    {
        private ProfileBuildingsData _buildingsData => session.profile.buildingsData;

        public TryCollectResourcesDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            var collectedResources = new Dictionary<ResourceType, int>();
            var notCollectedResources = new Dictionary<ResourceType, int>();

            var playerResources = session.player.resources;
            var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
            foreach (ProductionBuildingBase building in productionBuildings)
            {
                var farmedAmout = building.GetFarmedResourceAmount(_buildingsData);
                if (farmedAmout < 1)
                    continue;

                var reallyAdded = playerResources.Add(building.resourceType, farmedAmout);
                if (reallyAdded > 0)
                {
                    collectedResources.TryGetValue(building.resourceType, out var prevValue);
                    collectedResources[building.resourceType] = prevValue + reallyAdded;
                }

                if (reallyAdded == farmedAmout)
                {
                    building.SetStartFarmTime(_buildingsData, DateTime.UtcNow.Ticks);
                }
                else
                {
                    var startFarmDt = new DateTime(building.GetStartFarmTime(_buildingsData));
                    var totalFarmSeconds = (DateTime.UtcNow - startFarmDt).TotalSeconds;
                    var collectedPart = (float)reallyAdded / farmedAmout;
                    var secondsToRemove = totalFarmSeconds * collectedPart;
                    var newStartFarmDt = startFarmDt.AddSeconds(secondsToRemove);
                    building.SetStartFarmTime(_buildingsData, newStartFarmDt.Ticks);

                    var notCollectedAmount = farmedAmout - reallyAdded;
                    notCollectedResources.TryGetValue(building.resourceType, out var prevValue);
                    notCollectedResources[building.resourceType] = prevValue + notCollectedAmount;
                }
            }

            var sb = new StringBuilder();
            if (collectedResources.Count == 0 && notCollectedResources.Count == 0)
            {
                sb.AppendLine(Localization.Get(session, "dialog_buildings_no_resources_in_production"));
            }
            else
            {
                if (collectedResources.Count > 0)
                {
                    sb.AppendLine(Localization.Get(session, "dialog_buildings_collected_resources_header"));
                    sb.AppendLine(ResourceHelper.GetCompactResourcesView(collectedResources));
                    sb.AppendLine();
                }
                if (notCollectedResources.Count > 0)
                {
                    sb.AppendLine(Localization.Get(session, "dialog_buildings_resources_not_collected"));
                    sb.AppendLine();
                    sb.AppendLine(ResourceHelper.GetCompactResourcesView(notCollectedResources));
                }
            }

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => new BuildingsDialog(session).Start());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }
    }
}
