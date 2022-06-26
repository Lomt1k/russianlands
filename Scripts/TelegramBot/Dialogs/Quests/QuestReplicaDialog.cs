using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests
{
    internal class QuestReplicaDialog : DialogBase
    {
        private Replica _replica;

        public QuestReplicaDialog(GameSession _session, Replica replica) : base(_session)
        {
            _replica = replica;
            foreach (var answer in replica.answers)
            {
                var answerText = Localization.Get(session, answer.localizationKey);
                RegisterButton(answerText, () => OnGetAnswer(answer.nextStage));
            }
        }

        private string GetText()
        {
            var sb = new StringBuilder();
            if (_replica.characterType != CharacterType.None)
            {
                var characterName = _replica.characterType.GetNameBold(session);
                sb.AppendLine(characterName);
                sb.AppendLine();
            }
            sb.Append(Localization.Get(session, _replica.localizationKey));
            return sb.ToString();
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
            await messageSender.SendTextDialog(session.chatId, GetText(), GetMultilineKeyboard());
        }
    }
}
