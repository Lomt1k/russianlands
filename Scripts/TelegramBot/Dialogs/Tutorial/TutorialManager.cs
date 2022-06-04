using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public static class TutorialManager
    {
        public static async Task StartCurrentStage(GameSession session)
        {
            switch (session.profile.data.tutorialStage)
            {
                case 0:
                    await new TutorialSelectLanguageDialog(session).Start();
                    break;
                case 100:
                    await new TutorialEnterNameDialog(session).Start();
                    break;
                case 32000:
                    await new TutorialEndDialog(session).Start();
                    break;
            }
        }

        public static async Task StartNextStage(GameSession session)
        {
            var stage = session.profile.data.tutorialStage;
            switch (session.profile.data.tutorialStage)
            {
                case 0:
                    stage = 100;
                    break;
                case 100:
                    stage = 32000;
                    break;
            }

            session.profile.data.tutorialStage = stage;
            await StartCurrentStage(session);
        }

        public static async Task CompleteTutorial(GameSession session)
        {
            session.profile.data.tutorialStage = -1;
            await new TownEntryDialog(session, TownEntryReason.EndTutorial).Start();
        }

    }
}
