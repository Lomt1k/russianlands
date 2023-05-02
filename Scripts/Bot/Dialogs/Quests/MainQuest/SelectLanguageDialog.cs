using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot.Dialogs.Quests.MainQuest;

public class SelectLanguageDialog : DialogBase
{
    private static readonly DailyRemindersManager remindersManager = Services.Get<DailyRemindersManager>();

    public SelectLanguageDialog(GameSession _session) : base(_session)
    {
    }

    public override async Task Start()
    {
        var availableLanguages = BotController.config.languageCodes;
        if (availableLanguages.Length == 1)
        {
            await SetupLanguage(availableLanguages[0]).FastAwait();
            return;
        }

        var text = "Please select your language:\n";
        foreach (var languageCode in availableLanguages)
        {
            RegisterButton(languageCode.GetLanguageView(), () => SetupLanguage(languageCode));
        }

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    private async Task SetupLanguage(LanguageCode language)
    {
        session.profile.data.language = language;
        await remindersManager.ScheduleReminder(session.profile.data).FastAwait();
        await QuestManager.TryInvokeTrigger(session, TriggerType.InvokeFromCode).FastAwait();
    }
}
