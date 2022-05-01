using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEnterNameDialog : DialogBase
    {
        private string _fullName;

        public TutorialEnterNameDialog(User user)
        {
            _fullName = $"{user.FirstName} {user.LastName}";
        }

        protected override void Start()
        {
            SendEnterNameDialog();
        }

        private async void SendEnterNameDialog()
        {
            string text = "Привет, авантюрист! Как тебя зовут?\n\n" + "Введите имя в ответном сообщении. Так вас будут видеть другие игроки.";

            var row1 = new KeyboardButton[] { new KeyboardButton(session.profile.data.username) };
            var row2 = new KeyboardButton[] { new KeyboardButton(_fullName) };
            var markup = new KeyboardButton[][] { row1, row2 };
            var keyboard = new ReplyKeyboardMarkup(markup);

            await messageSender.SendTextDialog(chatId, text, replyKeyboard: keyboard);
        }

        public override void HandleMessage(User actualUser, Message message)
        {
            Program.logger.Debug($"HandleMessage in TutorialEnterNameDialog: {message.Text}");
        }


    }
}
