using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Quests
{
    public class QuestReplicaDialog : DialogBase
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
                var characterName = _replica.characterType.GetName(session).Bold();
                sb.AppendLine(characterName + ':');
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
            var quest = gameDataHolder.quests[focusedQuest.Value];
            await quest.SetStage(session, nextStageId);
        }

        public override async Task Start()
        {
            var sticker = _replica.characterType.GetSticker();
            if (sticker != null)
            {
                await messageSender.SendSticker(session.chatId, sticker, cancellationToken: session.cancellationToken);
            }
            await SendDialogMessage(GetText(), GetMultilineKeyboard()).FastAwait();
        }

        public override async Task TryResendDialog()
        {
            await Start().FastAwait();
        }

    }
}
