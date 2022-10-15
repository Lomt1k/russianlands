using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Quests.StageActions;
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
                await new TownDialog(session, TownEntryReason.StartNewSession).Start();
                return;
            }

            var focusedQuest = QuestsHolder.GetQuest(focusedQuestType.Value);
            var stageId = playerQuestsProgress.GetStage(focusedQuestType.Value);            
            var stage = focusedQuest.GetCurrentStage(session);

            var stageIdToSetup = stage.jumpToStageIfNewSession ?? stageId;

            if (focusedQuest.TryGetStageById(stageIdToSetup, out var stageToSetup))
            {
                switch (stageToSetup)
                {
                    // Если stage имеет вход в город - сразу вводим игрока в город с TownEntryReason.StartNewSession
                    case QuestStageWithTrigger stageWithTrigger:
                        bool hasTownEntry = stageWithTrigger.questActions.Where(x => x is EntryTownAction).Count() > 0;
                        if (hasTownEntry)
                        {
                            await new TownDialog(session, TownEntryReason.StartNewSession).Start();
                        }
                        break;
                    // Если игрок закончил игру на PvE точке (не вступив в бой) - также начинаем сессию с города
                    case QuestStageWithBattlePoint stageWithBattlePoint:
                        await new TownDialog(session, TownEntryReason.StartNewSession).Start();
                        break;
                }
            }

            await focusedQuest.SetStage(session, stageIdToSetup);
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
