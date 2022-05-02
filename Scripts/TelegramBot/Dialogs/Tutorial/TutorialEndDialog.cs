using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialEndDialog : DialogBase
    {
        public TutorialEndDialog(GameSession _session) : base(_session)
        {
        }

        protected override async void Start()
        {
            string localization = Localization.Get(session, "dialog_tutorial_end_text");
            string text = string.Format(localization, session.profile.data.nickname);
            string campButtonText = $"{Emojis.locations[Location.Camp]} " + Localization.Get(session, "location_camp");
            RegisterButton(campButtonText, OnCampButtonPressed);
            await messageSender.SendTextDialog(session.chatId, text, GetKeyboard());
        }

        private void OnCampButtonPressed()
        {
            TutorialManager.CompleteTutorial(session);
        }
    }
}
