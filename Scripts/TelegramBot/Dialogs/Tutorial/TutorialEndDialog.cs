using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEndDialog : DialogBase
    {
        public TutorialEndDialog(GameSession _session) : base(_session)
        {
        }

        public override async void Start()
        {
            string localization = Localization.Get(session, "dialog_tutorial_end_text");
            string text = string.Format(localization, session.profile.data.nickname);
            string campButtonText = $"{Emojis.townMenu[TownMenu.Town]} " + Localization.Get(session, "menu_item_town");
            RegisterButton(campButtonText, OnTownButtonPressed);
            await messageSender.SendTextDialog(session.chatId, text, GetOneLineKeyboard());
        }

        private void OnTownButtonPressed()
        {
            TutorialManager.CompleteTutorial(session);
        }
    }
}
