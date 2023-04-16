using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot.Dialogs.Quests.MainQuest
{
    public class SelectLanguageDialog : DialogBase
    {
        private static readonly DailyRemindersManager remindersManager = Services.Get<DailyRemindersManager>();

        public SelectLanguageDialog(GameSession _session) : base(_session)
        {
        }

        public override async Task Start()
        {
            await SetupLanguage(LanguageCode.RU);
            return; //заглушка: пока что тупо ставим русский язык

            string text = "Please select your language:\n" +
                "-----------------------\n" +
                "Пожалуйста, выберите ваш язык:";

            RegisterButton(Emojis.FlagBritain + "English", () => OnSelectEnglish());
            RegisterButton(Emojis.FlagRussia + "Русский", () => OnSelectRussian());

            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
        }

        private async Task OnSelectRussian()
        {
            await SetupLanguage(LanguageCode.RU).FastAwait();
        }

        private async Task OnSelectEnglish()
        {
            await SetupLanguage(LanguageCode.EN).FastAwait();
        }

        private async Task SetupLanguage(LanguageCode language)
        {
            session.profile.data.language = language;
            await remindersManager.ScheduleReminder(session.profile.data).FastAwait();
            await QuestManager.TryInvokeTrigger(session, TriggerType.InvokeFromCode).FastAwait();
        }
    }
}
