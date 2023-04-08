using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class MapDialogPanel : DialogPanelBase
    {
        public MapDialogPanel(DialogWithPanel _dialog) : base(_dialog)
        {
        }

        public override async Task Start()
        {
            await ShowGlobalMap()
                .ConfigureAwait(false);
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
            await SendPanelMessage(sb, GetKeyboardWithFixedRowSize(2))
                .ConfigureAwait(false);
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
            RegisterBackButton(() => ShowGlobalMap());
            await SendPanelMessage(sb, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

        public async Task ShowLocation(LocationType locationType)
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(locationType.GetLocalization(session).Bold());

            var questType = locationType.GetQuest();
            if (questType.HasValue)
            {
                var quest = QuestsHolder.GetQuest(questType.Value);
                if (quest.IsStarted(session) && !quest.IsCompleted(session))
                {
                    sb.AppendLine();
                    var currentProgress = quest.GetCompletedBattlePoints(session);
                    var totalProgress = quest.battlePointsCount;
                    var progressText = Localization.Get(session, "dialog_map_location_progress", currentProgress, totalProgress);
                    sb.AppendLine(Emojis.ButtonStoryMode + progressText);

                    RegisterButton(Emojis.ButtonStoryMode + Localization.Get(session, "dialog_map_continue_story_mode"),
                        () => ContinueStoryMode(locationType));
                }
            }
            RegisterBackButton(() => ShowGlobalMap());

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
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

            await QuestManager.TryInvokeTrigger(session, TriggerType.ContinueStoryMode)
                .ConfigureAwait(false);
        }

        // simulate Start() from BattlePointDialog
        private async Task SimulateStartBattlePointDialog(QuestStageWithBattlePoint stage, LocationType locationType)
        {
            var data = stage.GetMobBattlePointData(session);
            var text = Emojis.ButtonBattle + data.mob.GetFullUnitInfoView(session);

            ClearButtons();
            var priceView = data.foodPrice > 0 ? ResourceType.Food.GetEmoji() + data.foodPrice.View() : string.Empty;
            var startBattleButton = Localization.Get(session, "dialog_mob_battle_point_start_battle", priceView);
            RegisterButton(startBattleButton, () => new BattlePointDialog(session, data).SilentStart());
            RegisterBackButton(() => ShowLocation(locationType));
            RegisterBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, () => ShowGlobalMap());

            await SendPanelMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

    }
}
