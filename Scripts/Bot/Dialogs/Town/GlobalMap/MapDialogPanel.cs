using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Services.Mobs;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap;

public class MapDialogPanel : DialogPanelBase
{
    private static readonly LocationMobsManager locationMobsManager = Services.Get<LocationMobsManager>();

    private static GameDataDictionary<QuestId, QuestData> quests => gameDataHolder.quests;

    private MobDifficulty mobDifficulty => session.profile.dailyData.GetLocationMobDifficulty();

    public MapDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowGlobalMap().FastAwait();
    }

    private async Task ShowGlobalMap()
    {
        ClearButtons();
        var locations = Enum.GetValues(typeof(LocationId));
        foreach (LocationId locationId in locations)
        {
            if (locationId == LocationId.None)
                continue;

            var locationName = locationId.GetLocalization(session);
            if (locationId.IsLocked(session))
            {
                locationName = Emojis.ElementLocked + locationName;
                RegisterButton(locationName, () => ShowLockedLocationInfo(locationId));
                continue;
            }

            var questId = locationId.GetQuest();
            if (questId != null && quests.TryGetValue(questId.Value, out var quest))
            {
                var hasStory = quest.IsStarted(session) && !quest.IsCompleted(session);
                if (hasStory)
                {
                    RegisterButton(Emojis.ButtonStoryMode + locationName, () => ShowLocation(locationId));
                    continue;
                }
            }

            var mobsCount = locationMobsManager.isMobsReady ? locationMobsManager[mobDifficulty][locationId].Length : 0;
            var defeatedMobsCount = session.profile.dailyData.GetLocationDefeatedMobs(locationId).Count;
            var mobsRemaining = mobsCount - defeatedMobsCount;
            if (mobsRemaining <= 0)
            {
                RegisterButton(locationName, () => ShowLocation(locationId));
                continue;
            }

            var buttonText = new StringBuilder();
            var locationRewards = locationMobsManager.GetLocationRewards(session, locationId);
            foreach (var reward in locationRewards)
            {
                if (reward is ResourceReward resourceReward)
                {
                    buttonText.Append(resourceReward.resourceId.GetEmoji().ToString() + ' ');
                }
            }
            buttonText.Append(locationName);
            RegisterButton(buttonText.ToString(), () => ShowLocation(locationId));
        }

        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_map_select_location"));

        if (!LocationId.Loc_02.IsLocked(session))
        {
            if (locationMobsManager.isMobsReady)
            {
                sb.AppendLine();
                sb.AppendLine(locationMobsManager.GetTimerViewUntilMobsRespawn(session));
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_map_mobs_generation_in_progress"));
            }
        }

        TryAppendTooltip(sb);
        await SendPanelMessage(sb, GetKeyboardWithFixedRowSize(2)).FastAwait();
    }

    private async Task ShowLockedLocationInfo(LocationId locationId)
    {
        var sb = new StringBuilder();
        var locationName = locationId.GetLocalization(session);
        sb.AppendLine(Emojis.ElementLocked + locationName.Bold());

        sb.AppendLine();
        var previousLocation = (locationId - 1).GetLocalization(session);
        sb.AppendLine(Localization.Get(session, "dialog_map_location_locked", previousLocation));

        ClearButtons();
        RegisterBackButton(ShowGlobalMap);
        await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

    public async Task ShowLocation(LocationId locationId)
    {
        ClearButtons();
        var sb = new StringBuilder();
        sb.AppendLine(locationId.GetLocalization(session).Bold());

        var questId = locationId.GetQuest();
        var hasActiveQuest = false;
        if (questId.HasValue)
        {
            var quest = quests[questId.Value];
            hasActiveQuest = quest.IsStarted(session) && !quest.IsCompleted(session);
            if (hasActiveQuest)
            {
                AppendActiveQuestContent(sb, quest);
            }
        }

        if (!hasActiveQuest)
        {
            AppendLocationMobsContent(sb, locationId);
        }

        RegisterBackButton(ShowGlobalMap);

        TryAppendTooltip(sb);
        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private void AppendActiveQuestContent(StringBuilder sb, QuestData quest)
    {
        sb.AppendLine();
        var currentProgress = quest.GetCompletedBattlePoints(session);
        var totalProgress = quest.battlePointsCount;
        var progressText = Localization.Get(session, "dialog_map_location_progress", currentProgress, totalProgress);
        sb.AppendLine(Emojis.ButtonStoryMode + progressText);

        RegisterButton(Emojis.ButtonStoryMode + Localization.Get(session, "dialog_map_continue_story_mode"),
            () => ContinueStoryMode(quest.id.GetLocation().EnsureNotNull()));
    }

    private void AppendLocationMobsContent(StringBuilder sb, LocationId locationId)
    {
        sb.AppendLine();
        if (!locationMobsManager.isMobsReady)
        {
            sb.AppendLine(Localization.Get(session, "dialog_map_mobs_generation_in_progress"));
            return;
        }

        var mobDatas = locationMobsManager[mobDifficulty][locationId];
        var defeatedMobs = session.profile.dailyData.GetLocationDefeatedMobs(locationId);

        if (defeatedMobs.Count >= mobDatas.Length)
        {
            sb.AppendLine(Localization.Get(session, "dialog_map_location_all_enemies_defeated"));
            sb.AppendLine();
            sb.AppendLine(locationMobsManager.GetTimerViewUntilMobsRespawn(session));
            return;
        }

        sb.AppendLine(Localization.Get(session, "dialog_map_location_with_mobs_description"));
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_map_header_special_reward"));
        var locationRewards = locationMobsManager.GetLocationRewards(session, locationId);
        foreach (var reward in locationRewards)
        {
            if (reward is ResourceReward resourceReward)
            {
                var resourceData = new ResourceData(resourceReward.resourceId, resourceReward.amount);
                sb.AppendLine(resourceData.GetLocalizedView(session));
            }
        }

        for (byte i = 0; i < mobDatas.Length; i++)
        {
            if (defeatedMobs.Contains(i))
            {
                continue;
            }
            var mobData = mobDatas[i];
            var mobName = Localization.Get(session, mobData.localizationKey);
            var index = i; // it is important!
            RegisterButton(mobName, () => ShowBattlePointWithLocationMob(locationId, index));
        }
    }

    private async Task ContinueStoryMode(LocationId locationId)
    {
        var questId = locationId.GetQuest();
        if (questId == null)
            return;

        var quest = quests[questId.Value];
        if (quest == null || !quest.IsStarted(session))
            return;

        var stage = quest.GetCurrentStage(session);
        if (stage is QuestStageWithBattlePoint withBattlePoint)
        {
            await SimulateStartBattlePointDialog(withBattlePoint, locationId);
            return;
        }

        await QuestManager.TryInvokeTrigger(session, TriggerType.ContinueStoryMode).FastAwait();
    }

    // simulate Start() from BattlePointDialog
    private async Task SimulateStartBattlePointDialog(QuestStageWithBattlePoint stage, LocationId locationId)
    {
        var mobData = stage.GetMobBattlePointData(session);
        var text = Emojis.ButtonBattle + mobData.mob.GetFullUnitInfoView(session);

        ClearButtons();
        var priceView = mobData.foodPrice > 0 ? ResourceId.Food.GetEmoji() + mobData.foodPrice.View() : string.Empty;
        var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
        RegisterButton(startBattleButton, () => new BattlePointDialog(session, mobData).SilentStart());
        RegisterBackButton(() => ShowLocation(locationId));
        RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, ShowGlobalMap);

        await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task ShowBattlePointWithLocationMob(LocationId locationId, byte mobIndex)
    {
        var mobData = locationMobsManager.GetMobBattlePointData(session, locationId, mobIndex);
        if (mobData == null)
        {
            // Клик в момент пересоздания мобов
            var notification = Localization.Get(session, "dialog_map_mobs_generation_in_progress");
            await notificationsManager.ShowNotification(session, notification,
                () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BackFromInnerDialog)).FastAwait();
            return;
        }

        ClearButtons();
        var text = Emojis.ButtonBattle + mobData.mob.GetFullUnitInfoView(session);
        var priceView = mobData.foodPrice > 0 ? ResourceId.Food.GetEmoji() + mobData.foodPrice.View() : string.Empty;
        var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
        RegisterButton(startBattleButton, () => new BattlePointDialog(session, mobData).SilentStart());
        RegisterBackButton(() => ShowLocation(locationId));
        RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, ShowGlobalMap);

        await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

}
