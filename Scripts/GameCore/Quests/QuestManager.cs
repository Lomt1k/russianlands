using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    public class QuestManager
    {
        public static async Task HandleNewSession(GameSession session)
        {
            var playerQuestsProgress = session.profile.dynamicData.quests;
            var focusedQuestType = playerQuestsProgress.GetFocusedQuest();
            if (focusedQuestType == null)
            {
                if (!playerQuestsProgress.IsStarted(QuestType.MainQuest))
                {
                    await QuestsHolder.GetQuest(QuestType.MainQuest).StartQuest(session);
                    return;
                }
                await new TownEntryDialog(session, TownEntryReason.StartNewSession).Start();
                return;
            }

            var focusedQuest = QuestsHolder.GetQuest(focusedQuestType.Value);
            var stageId = playerQuestsProgress.GetStage(focusedQuestType.Value);            
            var stage = focusedQuest.GetCurrentStage(session);

            var stageToSetup = stage.jumpToStageIfNewSession ?? stageId;
            await focusedQuest.SetStage(session, stageToSetup);
            if (!focusedQuest.IsFocusRequired(session))
            {
                await new TownEntryDialog(session, TownEntryReason.StartNewSession).Start();
            }
        }

        public static async Task TryInvokeTrigger(GameSession session, TriggerType triggerType)
        {
            var focusedQuestType = session.profile.dynamicData.quests.GetFocusedQuest();
            if (focusedQuestType == null)
                return;

            var focusedQuest = QuestsHolder.GetQuest(focusedQuestType.Value);
            var stageId = session.profile.dynamicData.quests.GetStage(focusedQuestType.Value);            
            var stage = focusedQuest.GetCurrentStage(session);

            if (stage is QuestStageWithTrigger stageWithTrigger)
            {
                var trigger = stageWithTrigger.nextStageTriggers.First(x => x.triggerType == triggerType);
                bool success = trigger.TryInvoke();
                if (success)
                {
                    await focusedQuest.SetStage(session, trigger.nextStage);
                }
            }
        }

    }
}
