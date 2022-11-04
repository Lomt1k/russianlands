using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.MainQuest
{
    public class EnterNameDialog : DialogBase
    {
        private const int minLength = 3;
        private const int maxLength = 16;

        private bool _isQuestReplicaStage = false;

        public EnterNameDialog(GameSession _session) : base(_session)
        {
        }

        /*
         * Тут идёт реплика персонажа, но не через QuestReplicaStage, а костыльно
         * Сделано для того, чтобы пользователь мог либо:
         * - нажать на кнопку "Представиться" в релпике и тогда уже перейти к вводу имени
         * - сразу ввести имя, не нажимая кнопку "представиться"
         * Сделано чтобы избежать отвала игроков, когда они вместо нажатия на кнопку сразу вводят имя и ловят ступор
         */
        public async Task StartFromQuest()
        {
            _isQuestReplicaStage = true;
            var sticker = CharacterType.Vasilisa.GetSticker(Emotion.Idle);
            if (sticker != null)
            {
                await messageSender.SendSticker(session.chatId, sticker);
            }

            var text = Localization.Get(session, "quest_main_vasilisa_encounter_replica");
            var buttonText = GetQuestButtonText(session);
            RegisterButton(buttonText, null);
            await SendDialogMessage(text, GetOneLineKeyboard());
        }

        private string GetQuestButtonText(GameSession session)
        {
            return Localization.Get(session, "quest_main_vasilisa_encounter_answer");
        }

        public override async Task Start()
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_entry_name_description"));
            sb.AppendLine(Localization.Get(session, "dialog_entry_name_requirments_1"));
            sb.AppendLine(string.Format(Localization.Get(session, "dialog_entry_name_requirments_2"), minLength, maxLength));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_entry_name_header"));

            RegisterButton(session.player.nickname, null);
            await SendDialogMessage(sb, GetMultilineKeyboard());
        }

        public override async Task HandleMessage(Message message)
        {
            if (_isQuestReplicaStage)
            {
                if (string.IsNullOrWhiteSpace(message.Text) || message.Text.Equals(GetQuestButtonText(session)))
                {
                    _isQuestReplicaStage = false;
                    await Start();
                    return;
                }
            }

            if (message.Text == null || message.Text.Equals(GetQuestButtonText(session)))
                return;

            var nickname = message.Text;
            if (!nickname.IsCorrectNickname())
            {
                await Start();
                return;
            }

            session.profile.data.nickname = nickname;
            await QuestManager.TryInvokeTrigger(session, TriggerType.InvokeFromCode);
        }
    }
}
