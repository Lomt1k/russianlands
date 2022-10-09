using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.MainQuest
{
    public class EnterNameDialog : DialogBase
    {
        private const int minLength = 3;
        private const int maxLength = 16;

        public EnterNameDialog(GameSession _session) : base(_session)
        {
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
            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetMultilineKeyboard());
        }

        public override async Task HandleMessage(Message message)
        {
            await base.HandleMessage(message);

            if (message.Text == null)
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
