using System;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Commands;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Cheats
{
    public class CheatsDialog : DialogBase
    {
        public CheatsDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            ClearButtons();
            RegisterButton("Resources", () => ShowResourcesGroup());
            RegisterButton("Quest Progress", () => ShowQuestProgressGroup());
            RegisterTownButton(isDoubleBack: false);

            var header = "Cheats".Bold();
            await SendDialogMessage(header, GetKeyboardWithRowSizes(1, 1, 1))
                .ConfigureAwait(false);
        }

        #region Resources

        public async Task ShowResourcesGroup()
        {
            ClearButtons();
            foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
            {
                RegisterButton(resourceType.ToString(), () => SelectAmountForAddResource(resourceType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            await SendDialogMessage("Resources".Bold(), GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        public async Task SelectAmountForAddResource(ResourceType resourceType)
        {
            ClearButtons();
            RegisterButton("10", () => InvokeAddResourceCommand(resourceType, 10));
            RegisterButton("50", () => InvokeAddResourceCommand(resourceType, 50));
            RegisterButton("100", () => InvokeAddResourceCommand(resourceType, 100));
            RegisterButton("1k", () => InvokeAddResourceCommand(resourceType, 1_000));
            RegisterButton("10k", () => InvokeAddResourceCommand(resourceType, 10_000));
            RegisterButton("100k", () => InvokeAddResourceCommand(resourceType, 100_000));
            RegisterButton("1kk", () => InvokeAddResourceCommand(resourceType, 1_000_000));
            RegisterButton("10kk", () => InvokeAddResourceCommand(resourceType, 10_000_000));
            RegisterButton("MAX", () => InvokeAddResourceCommand(resourceType, int.MaxValue));
            RegisterBackButton("Resources", () => ShowResourcesGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{resourceType} | Add amount:";
            await SendDialogMessage(text, GetKeyboardWithRowSizes(3, 3, 3, 2))
                .ConfigureAwait(false);
        }

        public async Task InvokeAddResourceCommand(ResourceType resourceType, int amount)
        {
            await CommandHandler.HandleCommand(session, $"/addresource {resourceType} {amount}")
                .ConfigureAwait(false);
            await Start()
                .ConfigureAwait(false);
        }

        #endregion

        #region Quest Progress

        private async Task ShowQuestProgressGroup()
        {
            ClearButtons();
            foreach (QuestType questType in Enum.GetValues(typeof(QuestType)))
            {
                if (questType == QuestType.None)
                    continue;

                RegisterButton(questType.ToString(), () => SelectQuestStage(questType));
            }
            RegisterBackButton("Cheats", () => Start());
            RegisterTownButton(isDoubleBack: true);

            var sb = new StringBuilder();
            sb.AppendLine("Quest Progress".Bold());
            
            var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
            if (focusedQuest.HasValue)
            {
                sb.AppendLine();
                var quest = QuestsHolder.GetQuest(focusedQuest.Value);
                sb.AppendLine($"Current quest: {focusedQuest}");
                sb.AppendLine($"Current stage: {quest.GetCurrentStage(session).id}");
            }            

            await SendDialogMessage(sb.ToString(), GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SelectQuestStage(QuestType questType)
        {
            var quest = QuestsHolder.GetQuest(questType);
            if (quest == null)
                return;

            var stages = quest.stages;
            ClearButtons();
            foreach (var stage in stages)
            {
                var comment = stage.comment.Trim().Replace(Environment.NewLine, " ");
                var stageView = $"{stage.id} - {comment}";
                RegisterButton(stageView, () => SetupCurrentQuestProgress(questType, stage.id));
            }
            RegisterBackButton("Quest Progress", () => ShowQuestProgressGroup());
            RegisterDoubleBackButton("Cheats", () => Start());

            var text = $"{questType} | Set stage:";
            await SendDialogMessage(text, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task SetupCurrentQuestProgress(QuestType questType, int stageId)
        {
            var quest = QuestsHolder.GetQuest(questType);
            if (quest == null)
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"Quest progress changed".Bold());
            sb.AppendLine($"Current quest: {questType}");
            sb.AppendLine($"Current stage: {stageId}");
            await messageSender.SendTextMessage(session.chatId, sb.ToString())
                .ConfigureAwait(false);

            var playerQuestsProgress = session.profile.dynamicData.quests;
            playerQuestsProgress.Cheat_SetCurrentQuest(questType, stageId);
            await quest.SetStage(session, stageId)
                .ConfigureAwait(false);
        }

        #endregion

    }
}
