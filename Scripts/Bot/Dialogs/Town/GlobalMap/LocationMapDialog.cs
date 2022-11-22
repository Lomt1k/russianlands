using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.GlobalMap
{
    public class LocationMapDialog : DialogBase
    {
        private LocationType _locationType;

        public LocationMapDialog(GameSession _session, LocationType locationType) : base(_session)
        {
            _locationType = locationType;
        }

        public override async Task Start()
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{_locationType.GetLocalization(session)}</b>");

            var questType = _locationType.GetQuest();
            if (questType.HasValue)
            {
                var quest = QuestsHolder.GetQuest(questType.Value);
                if (quest.IsStarted(session) && !quest.IsCompleted(session))
                {
                    sb.AppendLine();
                    var currentProgress = quest.GetCompletedBattlePoints(session);
                    var totalProgress = quest.battlePointsCount;
                    var progressText = string.Format(Localization.Get(session, "dialog_map_location_progress"), currentProgress, totalProgress);
                    sb.AppendLine($"{Emojis.locations[MapLocation.StoryMode]} " + progressText);

                    RegisterButton($"{Emojis.locations[MapLocation.StoryMode]} {Localization.Get(session, "dialog_map_continue_story_mode")}",
                        () => ContinueStoryMode());
                }
            }
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_map")} {Emojis.menuItems[MenuItem.Map]}",
                () => new GlobalMapDialog(session).Start());
            RegisterTownButton(isFullBack: true);

            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task ContinueStoryMode()
        {
            var questType = _locationType.GetQuest();
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
