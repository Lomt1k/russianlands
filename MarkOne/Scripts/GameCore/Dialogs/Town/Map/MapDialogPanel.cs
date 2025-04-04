﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Quests.NextStageTriggers;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Services.Mobs;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map;

public class MapDialogPanel : DialogPanelBase
{
    private static readonly LocationMobsManager locationMobsManager = ServiceLocator.Get<LocationMobsManager>();

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
        sb.AppendLine(locationRewards.GetPossibleRewardsView(session));

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
        var priceView = mobData.price.amount > 0 ? mobData.price.resourceId.GetEmoji() + mobData.price.amount.View() : string.Empty;
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
        var priceView = mobData.price.amount > 0 ? mobData.price.resourceId.GetEmoji() + mobData.price.amount.View() : string.Empty;
        var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
        RegisterButton(startBattleButton, () => CheckFreeInvetorySpaceAndStartBattle(locationId, mobData));
        RegisterBackButton(() => ShowLocation(locationId));
        RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, ShowGlobalMap);

        await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task CheckFreeInvetorySpaceAndStartBattle(LocationId locationId, BattlePointData battlePointData)
    {
        if (!locationMobsManager.isMobsReady)
        {
            // Клик в момент пересоздания мобов
            var notification = Localization.Get(session, "dialog_map_mobs_generation_in_progress");
            await notificationsManager.ShowNotification(session, notification,
                () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BackFromInnerDialog)).FastAwait();
            return;
        }

        var mobDatas = locationMobsManager[mobDifficulty][locationId];
        var defeatedMobs = session.profile.dailyData.GetLocationDefeatedMobs(locationId);
        var isLastMob = mobDatas.Length - defeatedMobs.Count == 1;

        if (isLastMob)
        {
            var playerResources = session.player.resources;
            var freeItemSlots = playerResources.GetResourceLimit(ResourceId.InventoryItems) - playerResources.GetValue(ResourceId.InventoryItems);
            var locationRewards = locationMobsManager.GetLocationRewards(session, locationId);
            var itemRewardsCount = battlePointData.rewards.GetInventoryItemsCount();
            itemRewardsCount += locationRewards.GetInventoryItemsCount();
            if (itemRewardsCount > freeItemSlots)
            {
                var text = Localization.Get(session, "dialog_mob_battle_point_inventory_slots_required", itemRewardsCount);
                await new SimpleDialog(session, text, withTownButton: false, new()
                {
                    { Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory"), () => new InventoryDialog(session).Start() },
                    { Emojis.ElementBack + Localization.Get(session, "menu_item_back_button"), () => new MapDialog(session).StartWithLocation(locationId) },
                }).Start().FastAwait();
                return;
            }
        }

        await new BattlePointDialog(session, battlePointData).SilentStart().FastAwait();
    }

}
