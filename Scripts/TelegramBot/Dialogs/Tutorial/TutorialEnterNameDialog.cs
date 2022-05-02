using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEnterNameDialog : DialogBase
    {
        public TutorialEnterNameDialog(GameSession _session) : base(_session)
        {
        }

        protected override void Start()
        {
            SendEnterNameDialog();
        }

        private async void SendEnterNameDialog()
        {
            string text = "Привет, авантюрист! Как тебя зовут?\n\n" + "Введите имя в ответном сообщении. Так вас будут видеть другие игроки.";

            string fullname = $"{session.actualUser.FirstName} {session.actualUser.LastName}";
            var row1 = new KeyboardButton[] { new KeyboardButton(fullname) };
            var row2 = new KeyboardButton[] { new KeyboardButton(session.profile.data.username) };
            var markup = new KeyboardButton[][] { row1, row2 };
            var keyboard = new ReplyKeyboardMarkup(markup);

            await messageSender.SendTextDialog(session.chatId, text, replyKeyboard: keyboard);
        }

        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
        }


    }
}
