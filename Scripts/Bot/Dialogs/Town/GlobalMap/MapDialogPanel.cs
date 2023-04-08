using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class MapDialogPanel : DialogPanelBase
    {
        public MapDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowGeneralMap()
                .ConfigureAwait(false);
        }

        private async Task ShowGeneralMap()
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
            RegisterBackButton(() => ShowGeneralMap());
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
            RegisterBackButton(() => ShowGeneralMap());

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
                await withBattlePoint.InvokeStage(session);
                return;
            }

            await QuestManager.TryInvokeTrigger(session, TriggerType.ContinueStoryMode)
                .ConfigureAwait(false);
        }

    }
}
