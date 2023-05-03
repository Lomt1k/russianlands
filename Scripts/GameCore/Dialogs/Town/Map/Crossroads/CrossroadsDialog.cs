using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Mobs;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Crossroads;

public class CrossroadsDialog : DialogBase
{
    private static readonly CrossroadsMobsManager crossroadsMobsManager = ServiceLocator.Get<CrossroadsMobsManager>();

    public CrossroadsDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        if (!crossroadsMobsManager.isMobsReady)
        {
            var notification = Localization.Get(session, "dialog_map_mobs_generation_in_progress");
            await notificationsManager.ShowNotification(session, notification,
                () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BackFromInnerDialog)).FastAwait();
            return;
        }

        var difficulty = MobDifficultyCalculator.GetActualDifficultyForPlayer(session.player);
        var crossId = session.profile.dailyData.lastCrossroadId + 1;
        var mobs = crossroadsMobsManager[difficulty][crossId];
        var energyInfo = ResourceHelper.RefreshCrossroadsEnergy(session);

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

    private async Task ShowMobPoint(CrossroadsMobData mobData)
    {
        var battlePointData = GetMobBattlePointData(mobData);
        await new CrossroadsBattlePointDialog(session, battlePointData).Start().FastAwait();
    }

    public BattlePointData GetMobBattlePointData(CrossroadsMobData mobData)
    {
        var reward = new ResourceReward() { resourceId = mobData.fruitId, amount = 1 };

        return new BattlePointData
        {
            mob = new Mob(session, mobData),
            price = new ResourceData(ResourceId.CrossroadsEnergy, 1),
            rewards = new RewardBase[] { reward },
            onBackButtonFunc = () => new CrossroadsDialog(session).Start(),
            onBattleEndFunc = (Player player, BattleResult result) =>
            {
                player.session.profile.dailyData.lastCrossroadId++;
                return Task.CompletedTask;
            },
            onContinueButtonFunc = async (Player player, BattleResult result) =>
            {
                await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BattleEnd).FastAwait();
            },
        };
    }

}
