using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Quests.MainQuest
{
    public class SelectLanguageDialog : DialogBase
    {
        public SelectLanguageDialog(GameSession _session) : base(_session)
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
            await QuestManager.TryInvokeTrigger(session, TriggerType.InvokeFromCode);
        }
    }
}
