using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using MarkOne.Scripts.Bot.Dialogs.Town;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Quests.NextStageTriggers;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Quests.StageActions;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;

namespace MarkOne.Scripts.GameCore.Quests;

public class QuestManager
{
    private static readonly GameDataHolder gameDataHolder = Services.Services.Get<GameDataHolder>();
    private static readonly NotificationsManager notificationsManager = Services.Services.Get<NotificationsManager>();

    public static async Task HandleNewSession(GameSession session, Update update)
    {
        var playerQuestsProgress = session.profile.dynamicData.quests;
        var focusedQuestId = playerQuestsProgress.GetFocusedQuest();
        if (focusedQuestId == null)
        {
            if (!playerQuestsProgress.IsStarted(QuestId.MainQuest))
            {
                await gameDataHolder.quests[QuestId.MainQuest].StartQuest(session).FastAwait();
                return;
            }
            await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.StartNewSession).FastAwait();
            return;
        }

        var focusedQuest = gameDataHolder.quests[focusedQuestId.Value];
        var stageId = playerQuestsProgress.GetStage(focusedQuestId.Value);
        var stage = focusedQuest.GetCurrentStage(session);

        var stageIdToSetup = stage.jumpToStageIfNewSession ?? stageId;
        var isJumped = stage.jumpToStageIfNewSession.HasValue;

        if (!isJumped && focusedQuest.TryGetStageById(stageIdToSetup, out var stageToSetup))
        {
            var replyMessage = update.Message?.Text ?? string.Empty;
            switch (stageToSetup)
            {
                // Если stage имеет вход в город - сразу вводим игрока в город с TownEntryReason.StartNewSession
                case QuestStageWithTrigger stageWithTrigger:
                    var hasTownEntry = stageWithTrigger.questActions.Where(x => x is EntryTownAction).Count() > 0;
                    if (hasTownEntry)
                    {
                        await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.StartNewSession).FastAwait();
                    }
                    return;

                // Если игрок закончил игру на PvE точке и в начале новой сессии нажал "в бой" - запускаем бой
                case QuestStageWithBattlePoint stageWithBattlePoint:
                    var isStartBattlePressed = replyMessage.Contains(ResourceId.Food.GetEmoji().ToString());
                    if (isStartBattlePressed)
                    {
                        await stageWithBattlePoint.InvokeStageWithStartBattleImmediate(session).FastAwait();
                    }
                    else
                    {
                        await notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.StartNewSession).FastAwait();
                    }
                    return;

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

        await focusedQuest.SetStage(session, stageIdToSetup).FastAwait();
    }

    public static async Task TryInvokeTrigger(GameSession session, TriggerType triggerType)
    {
        var focusedQuestId = session.profile.dynamicData.quests.GetFocusedQuest();
        if (focusedQuestId == null)
            return;

        var focusedQuest = gameDataHolder.quests[focusedQuestId.Value];
        var stageId = session.profile.dynamicData.quests.GetStage(focusedQuestId.Value);
        var stage = focusedQuest.GetCurrentStage(session);

        if (stage is QuestStageWithTrigger stageWithTrigger)
        {
            var trigger = stageWithTrigger.nextStageTriggers.First(x => x.triggerType == triggerType);
            var success = trigger.TryInvoke();
            if (success)
            {
                await focusedQuest.SetStage(session, trigger.nextStage).FastAwait();
            }
        }
    }

}
