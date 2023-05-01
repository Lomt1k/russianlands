using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.Mobs;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class MapDialogPanel : DialogPanelBase
    {
        private static readonly LocationMobsManager locationMobsManager = Services.Get<LocationMobsManager>();

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
            var locations = Enum.GetValues(typeof(LocationType));
            foreach (LocationType locationType in locations)
            {
                if (locationType == LocationType.None)
                    continue;

                string locationName = locationType.GetLocalization(session);
                if (locationType.IsLocked(session))
                {
                    locationName = Emojis.ElementLocked + locationName;
                    RegisterButton(locationName, () => ShowLockedLocationInfo(locationType));
                    continue;
                }

                var questType = locationType.GetQuest();
                var quest = questType.HasValue ? QuestsHolder.GetQuest(questType.Value) : null;
                var hasStory = quest != null && quest.IsStarted(session) && !quest.IsCompleted(session);
                var prefix = hasStory ? Emojis.ButtonStoryMode : Emojis.Empty;

                RegisterButton(prefix + locationName, () => ShowLocation(locationType));
            }

            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, "dialog_map_select_location"));
            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetKeyboardWithFixedRowSize(2)).FastAwait();
        }

        private async Task ShowLockedLocationInfo(LocationType locationType)
        {
            var sb = new StringBuilder();
            var locationName = locationType.GetLocalization(session);
            sb.AppendLine(Emojis.ElementLocked + locationName.Bold());

            sb.AppendLine();
            var previousLocation = (locationType - 1).GetLocalization(session);
            sb.AppendLine(Localization.Get(session, "dialog_map_location_locked", previousLocation));

            ClearButtons();
            RegisterBackButton(ShowGlobalMap);
            await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
        }

        public async Task ShowLocation(LocationType locationType)
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(locationType.GetLocalization(session).Bold());

            var questType = locationType.GetQuest();
            var hasActiveQuest = false;
            if (questType.HasValue)
            {
                var quest = QuestsHolder.GetQuest(questType.Value);
                hasActiveQuest = quest.IsStarted(session) && !quest.IsCompleted(session);
                if (hasActiveQuest)
                {
                    AppendActiveQuestContent(sb, quest);
                }
            }

            if (!hasActiveQuest)
            {
                AppendLocationMobsContent(sb, locationType);
            }

            RegisterBackButton(ShowGlobalMap);

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private void AppendActiveQuestContent(StringBuilder sb, Quest quest)
        {
            sb.AppendLine();
            var currentProgress = quest.GetCompletedBattlePoints(session);
            var totalProgress = quest.battlePointsCount;
            var progressText = Localization.Get(session, "dialog_map_location_progress", currentProgress, totalProgress);
            sb.AppendLine(Emojis.ButtonStoryMode + progressText);

            RegisterButton(Emojis.ButtonStoryMode + Localization.Get(session, "dialog_map_continue_story_mode"),
                () => ContinueStoryMode(quest.questType.GetLocation().EnsureNotNull()));
        }

        private void AppendLocationMobsContent(StringBuilder sb, LocationType locationType)
        {
            if (!locationMobsManager.isMobsReady)
            {
                return;
            }
            var locationDefeatedMobs = session.profile.dailyData.GetLocationDefeatedMobs(locationType);

            sb.AppendLine();
            // TODO text
            
            var mobDatas = locationMobsManager[mobDifficulty][locationType];
            for (byte i = 0; i < mobDatas.Length; i++)
            {
                if (locationDefeatedMobs.Contains(i))
                {
                    continue;
                }
                var mobData = mobDatas[i];
                var mobName = Localization.Get(session, mobData.localizationKey);
                var index = i; // it is important!
                RegisterButton(mobName, () => ShowBattlePointWithLocationMob(locationType, index));
            }
        }

        private async Task ContinueStoryMode(LocationType locationType)
        {
            var questType = locationType.GetQuest();
            if (questType == null)
                return;

            var quest = QuestsHolder.GetQuest(questType.Value);
            if (quest == null || !quest.IsStarted(session))
                return;

            var stage = quest.GetCurrentStage(session);
            if (stage is QuestStageWithBattlePoint withBattlePoint)
            {
                await SimulateStartBattlePointDialog(withBattlePoint, locationType);
                return;
            }

            await QuestManager.TryInvokeTrigger(session, TriggerType.ContinueStoryMode).FastAwait();
        }

        // simulate Start() from BattlePointDialog
        private async Task SimulateStartBattlePointDialog(QuestStageWithBattlePoint stage, LocationType locationType)
        {
            var mobData = stage.GetMobBattlePointData(session);
            var text = Emojis.ButtonBattle + mobData.mob.GetFullUnitInfoView(session);

            ClearButtons();
            var priceView = mobData.foodPrice > 0 ? ResourceId.Food.GetEmoji() + mobData.foodPrice.View() : string.Empty;
            var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
            RegisterButton(startBattleButton, () => new BattlePointDialog(session, mobData).SilentStart());
            RegisterBackButton(() => ShowLocation(locationType));
            RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, ShowGlobalMap);

            await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
        }

        private async Task ShowBattlePointWithLocationMob(LocationType locationType, byte mobIndex)
        {
            var mobData = locationMobsManager.GetMobBattlePointData(session, locationType, mobIndex);
            if (mobData == null)
            {
                // Клик в момент пересоздания мобов (можно здесь вывести игроку какое-то сообщение
                return;
            }

            ClearButtons();
            var text = Emojis.ButtonBattle + mobData.mob.GetFullUnitInfoView(session);
            var priceView = mobData.foodPrice > 0 ? ResourceId.Food.GetEmoji() + mobData.foodPrice.View() : string.Empty;
            var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
            RegisterButton(startBattleButton, () => new BattlePointDialog(session, mobData).SilentStart());
            RegisterBackButton(() => ShowLocation(locationType));
            RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, ShowGlobalMap);

            await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack()).FastAwait();
        }

    }
}
