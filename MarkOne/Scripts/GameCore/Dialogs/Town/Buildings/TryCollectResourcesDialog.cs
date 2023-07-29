using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Buildings;

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
        Program.logger.Info($"User {session.actualUser} collected resources from production buildings");
        var collectedResources = new Dictionary<ResourceId, int>();
        var notCollectedResources = new Dictionary<ResourceId, int>();

        var playerResources = session.player.resources;
        var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
        var buildingsData = session.profile.buildingsData;
        var dtNow = DateTime.UtcNow;
        foreach (ProductionBuildingBase building in productionBuildings)
        {
            building.UpdateProduction(buildingsData);
            var storageAmount = building.GetStorageResourceAmount(buildingsData);
            if (storageAmount < 1)
            {
                continue;
            }

            var reallyAdded = playerResources.Add(new ResourceData(building.resourceId, storageAmount));
            if (reallyAdded.amount > 0)
            {
                collectedResources.TryGetValue(building.resourceId, out var prevValue);
                collectedResources[building.resourceId] = prevValue + reallyAdded.amount;
            }

            building.SetStorageResourceAmount(buildingsData, storageAmount - reallyAdded.amount);
            building.SetStartFarmTime(buildingsData, dtNow);
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

        buildingsData.lastResourceCollectTime = dtNow;
        return sb.ToString();
    }

}
