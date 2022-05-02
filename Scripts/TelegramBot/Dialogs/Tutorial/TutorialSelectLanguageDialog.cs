using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialSelectLanguageDialog : DialogBase
    {
        public TutorialSelectLanguageDialog(GameSession _session) : base(_session)
        {
        }

        protected async override void Start()
        {
            string text = "Пожалуйста, выберите ваш язык:\n" +
                "-----------------------\n" +
                "Please select your language:";

            RegisterButton($"{Emojis.flags[Flag.Russia]} Русский", OnSelectRussian);
            RegisterButton($"{Emojis.flags[Flag.GreatBritain]} English", OnSelectEnglish);

            await messageSender.SendTextDialog(session.chatId, text, GetKeyboardMultiline());
        }

        private void OnSelectRussian()
        {
            SetupLanguage(LanguageCode.ru);
        }

        private void OnSelectEnglish()
        {
            SetupLanguage(LanguageCode.en);
        }

        private void SetupLanguage(LanguageCode language)
        {
            session.profile.data.language = language.ToString();
            session.SetupLanguage(language);
            TutorialManager.StartNextStage(session);
        }
    }
}
