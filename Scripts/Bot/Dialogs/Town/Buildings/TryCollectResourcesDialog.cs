using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
{
    public class TryCollectResourcesDialog : DialogBase
    {
        public TryCollectResourcesDialog(GameSession _session, Func<Task> onContinue) : base(_session)
        {
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), onContinue);
        }

        public override async Task Start()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_buildings_force_collected_resources"));
            sb.AppendLine();

            var message = StartLogicAndGetResultMessage(session);
            sb.AppendLine(message);
            await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
        }

        public static string StartLogicAndGetResultMessage(GameSession session)
        {
            var collectedResources = new Dictionary<ResourceId, int>();
            var notCollectedResources = new Dictionary<ResourceId, int>();

            var playerResources = session.player.resources;
            var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
            var buildingsData = session.profile.buildingsData;
            foreach (ProductionBuildingBase building in productionBuildings)
            {
                var farmedAmout = building.GetFarmedResourceAmount(buildingsData);
                if (farmedAmout < 1)
                    continue;

                var reallyAdded = playerResources.Add(new ResourceData(building.resourceId, farmedAmout));
                if (reallyAdded.amount > 0)
                {
                    collectedResources.TryGetValue(building.resourceId, out var prevValue);
                    collectedResources[building.resourceId] = prevValue + reallyAdded.amount;
                }

                if (reallyAdded.amount == farmedAmout)
                {
                    building.SetStartFarmTime(buildingsData, DateTime.UtcNow);
                }
                else
                {
                    var startFarmDt = building.GetStartFarmTime(buildingsData);
                    var totalFarmSeconds = (DateTime.UtcNow - startFarmDt).TotalSeconds;
                    var collectedPart = (float)reallyAdded.amount / farmedAmout;
                    var secondsToRemove = totalFarmSeconds * collectedPart;
                    var newStartFarmDt = startFarmDt.AddSeconds(secondsToRemove);
                    building.SetStartFarmTime(buildingsData, newStartFarmDt);

                    var notCollectedAmount = farmedAmout - reallyAdded.amount;
                    notCollectedResources.TryGetValue(building.resourceId, out var prevValue);
                    notCollectedResources[building.resourceId] = prevValue + notCollectedAmount;
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
                    var collectedDatas = new List<ResourceData>();
                    foreach (var (resourceId, amount) in collectedResources)
                    {
                        collectedDatas.Add(new ResourceData(resourceId, amount));
                    }

                    sb.AppendLine(Localization.Get(session, "dialog_buildings_collected_resources_header"));
                    sb.AppendLine(collectedDatas.GetCompactView());
                    sb.AppendLine();
                }

                if (notCollectedResources.Count > 0)
                {
                    var notCollectedDatas = new List<ResourceData>();
                    foreach (var (resourceId, amount) in notCollectedResources)
                    {
                        notCollectedDatas.Add(new ResourceData(resourceId, amount));
                    }

                    sb.AppendLine(Localization.Get(session, "dialog_buildings_resources_not_collected"));
                    sb.AppendLine();
                    sb.AppendLine(notCollectedDatas.GetCompactView());
                }
            }
            return sb.ToString();
        }

    }
}
