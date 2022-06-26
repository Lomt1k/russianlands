using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    internal class QuestReplicaDialog : DialogBase
    {
        private string _text;

        public QuestReplicaDialog(GameSession _session, Replica replica) : base(_session)
        {
            _text = Localization.Get(session, replica.localizationKey);

            foreach (var answer in replica.answers)
            {
                var answerText = Localization.Get(session, answer.localizationKey);
                RegisterButton(answerText, () => OnGetAnswer(answer.nextStage));
            }
        }

        private async Task OnGetAnswer(int nextStageId)
        {
            var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
            if (focusedQuest == null)
            {
                Program.logger.Error("Can`t get answer because focusedQuest is NULL!");
                return;
            }
            var quest = QuestsHolder.GetQuest(focusedQuest.Value);
            await quest.SetStage(session, nextStageId);
        }

        public override async Task Start()
        {
            await messageSender.SendTextDialog(session.chatId, _text, GetMultilineKeyboard());
        }
    }
}
