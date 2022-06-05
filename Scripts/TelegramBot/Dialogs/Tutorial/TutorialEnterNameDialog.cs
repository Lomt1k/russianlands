using Telegram.Bot.Types;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Utils;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEnterNameDialog : DialogBase
    {
        private const int minNameLength = 2;
        private const int maxNameLength = 20;

        public TutorialEnterNameDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            string text = Localization.Get(session, "dialog_tutorial_enterName_text");

            string fullname = $"{session.actualUser.FirstName} {session.actualUser.LastName}";
            RegisterButton(fullname, null);
            RegisterButton(session.profile.data.username, null);

            await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
        }

        public override async Task HandleMessage(Message message)
        {
            await base.HandleMessage(message);

            if (message.Text == null)
                return;

            var nickname = message.Text;
            if (nickname.Length < minNameLength || nickname.Length > maxNameLength)
            {
                string localization = Localization.Get(session, "dialog_tutorial_enterName_incorrectLength");
                var formattedText = string.Format(localization, minNameLength, maxNameLength);
                await messageSender.SendTextMessage(session.chatId, formattedText);
                return;
            }
            if (!nickname.IsCorrectNickname())
            {
                string localization = Localization.Get(session, "dialog_tutorial_enterName_incorrectSymbols");
                await messageSender.SendTextMessage(session.chatId, localization);
                return;
            }

            session.profile.data.nickname = nickname;
            await TutorialManager.StartNextStage(session);
        }


    }
}
