using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Mobs;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Mobs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Crossroads;

public class CrossroadsDialog : DialogBase
{
    private const int SECONDS_FOR_ENERGY = 7200;

    private static readonly CrossroadsMobsManager crossroadsMobsManager = ServiceLocator.Get<CrossroadsMobsManager>();

    public CrossroadsDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        var difficulty = MobDifficultyCalculator.GetActualDifficultyForPlayer(session.player);
        var crossId = session.profile.dailyData.lastCrossroadId + 1;
        var mobs = crossroadsMobsManager[difficulty][crossId];
        var energyInfo = RefreshEnergyInfo();

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "menu_item_crossroads").Bold())
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_crossroads_description"))
            .AppendLine()
            .AppendLine(GetStoneView(mobs))
            .AppendLine()
            .AppendLine(Localization.Get(session, "dialog_crossroads_your_energy_header"))
            .AppendLine(Localization.Get(session, "dialog_crossroads_energy_view", energyInfo.resourceData.GetLocalizedView(session), energyInfo.resourceLimit));

        if (energyInfo.resourceData.amount < energyInfo.resourceLimit)
        {
            sb.AppendLine();
            var emoji = ResourceId.CrossroadsEnergy.GetEmoji().ToString();
            var timer = energyInfo.timeUntilNextEnergy.GetView(session);            
            sb.Append(Localization.Get(session, "dialog_crossroads_next_energy_info", emoji, timer));
        }

        RegisterButton(mobs[0].fruitId.GetEmoji() + Localization.Get(session, "dialog_crossroads_left_way_button"), () => ShowMobPoint(mobs[0]));
        RegisterButton(mobs[1].fruitId.GetEmoji() + Localization.Get(session, "dialog_crossroads_forward_way_button"), () => ShowMobPoint(mobs[1]));
        RegisterButton(mobs[2].fruitId.GetEmoji() + Localization.Get(session, "dialog_crossroads_right_way_button"), () => ShowMobPoint(mobs[2]));
        RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, () => new MapDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 2)).FastAwait();
    }

    private string GetStoneView(CrossroadsMobData[] mobs)
    {
        var fruitViews = new string[mobs.Length];
        for (int i = 0; i < fruitViews.Length; i++)
        {
            var fruitId = mobs[i].fruitId;
            fruitViews[i] = Localization.Get(session, $"dialog_crossroads_{fruitId.ToString().ToLower()}");
        }

        return new StringBuilder()
            .AppendLine(Localization.Get(session, "dialog_crossroads_stone_header"))
            .AppendLine(Localization.Get(session, "dialog_crossroads_left_way", fruitViews[0]))
            .AppendLine(Localization.Get(session, "dialog_crossroads_forward_way", fruitViews[1]))
            .Append(Localization.Get(session, "dialog_crossroads_right_way", fruitViews[2]))
            .ToString();
    }

    private (ResourceData resourceData, int resourceLimit, TimeSpan timeUntilNextEnergy) RefreshEnergyInfo()
    {
        var playerResources = session.player.resources;
        var now = DateTime.UtcNow;
        var lastUpdateTime = session.profile.data.lastCrossroadsResourceUpdate;
        var nextTime = lastUpdateTime;
        int energyToAdd = 0;
        while (nextTime <= now)
        {
            nextTime = lastUpdateTime.AddSeconds(SECONDS_FOR_ENERGY);
            if (nextTime <= now)
            {
                energyToAdd++;
                lastUpdateTime = nextTime;
            }
        }
        session.profile.data.lastCrossroadsResourceUpdate = lastUpdateTime;
        playerResources.Add(new ResourceData(ResourceId.CrossroadsEnergy, energyToAdd));
        var resourceData = playerResources.GetResourceData(ResourceId.CrossroadsEnergy);
        var resourceLimit = playerResources.GetResourceLimit(ResourceId.CrossroadsEnergy);
        return (resourceData, resourceLimit, nextTime - now);
    }

    private async Task ShowMobPoint(CrossroadsMobData mobData)
    {

    }

}
