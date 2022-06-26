﻿using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public class TutorialSelectLanguageDialog : DialogBase
    {
        public TutorialSelectLanguageDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            string text = "Please select your language:\n" +
                "-----------------------\n" +
                "Пожалуйста, выберите ваш язык:";

            RegisterButton($"{Emojis.flags[Flag.GreatBritain]} English", () => OnSelectEnglish());
            RegisterButton($"{Emojis.flags[Flag.Russia]} Русский", () => OnSelectRussian());

            await messageSender.SendTextDialog(session.chatId, text, GetMultilineKeyboard());
        }

        private async Task OnSelectRussian()
        {
            await SetupLanguage(LanguageCode.ru);
        }

        private async Task OnSelectEnglish()
        {
            await SetupLanguage(LanguageCode.en);
        }

        private async Task SetupLanguage(LanguageCode language)
        {
            session.profile.data.language = language.ToString();
            session.SetupLanguage(language);
            await TutorialManager.StartNextStage(session);
        }
    }
}
