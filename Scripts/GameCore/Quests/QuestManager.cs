using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Quests.StageActions;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    public class QuestManager
    {
        public static async Task HandleNewSession(GameSession session, Update update)
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
            bool isJumped = stage.jumpToStageIfNewSession.HasValue;

            if (!isJumped && focusedQuest.TryGetStageById(stageIdToSetup, out var stageToSetup))
            {
                string replyMessage = update.Message?.Text ?? string.Empty;
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

                    // Если игрок закончил игру на PvE точке и в начале новой сессии нажал "в бой" - запускаем бой
                    case QuestStageWithBattlePoint stageWithBattlePoint:
                        bool isStartBattlePressed = replyMessage.Contains(Emojis.resources[Resources.ResourceType.Food]);
                        if (isStartBattlePressed)
                        {
                            await stageWithBattlePoint.InvokeStageWithStartBattleImmediate(session);
                        }
                        else
                        {
                            await new TownDialog(session, TownEntryReason.StartNewSession).Start();
                        }                        
                        break;

                    // Если игрок прислал корректный вариант ответа в диалоге - переходим на следующий этап диалога
                    case QuestStageWithReplica stageWithReplica:
                        foreach (var answer in stageWithReplica.replica.answers)
                        {
                            if (answer.IsReplyMessageEquals(session, replyMessage))
                            {
                                stageIdToSetup = answer.nextStage;
                                break;
                            }
                        }
                        break;

                    // У Default Replica всегда один вариант ответа
                    case QuestStageWithDefaultReplica stageWithDefaultReplica:
                        stageIdToSetup = stageWithDefaultReplica.replica.answers[0].nextStage;
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
