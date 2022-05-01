using Telegram.Bot.Types;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEnterNameDialog : DialogBase
    {
        protected override void Start()
        {
            messageSender.SendTextDialog(chatId, "Привет, герой! Как тебя зовут?");
        }

        public override void HandleMessage(User actualUser, Message message)
        {
            Program.logger.Debug($"HandleMessage in dialog: {message.Text}");
        }


    }
}
