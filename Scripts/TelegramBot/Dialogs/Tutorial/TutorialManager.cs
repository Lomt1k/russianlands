using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public static class TutorialManager
    {
        public static void StartCurrentStage(GameSession session)
        {
            switch (session.profile.data.tutorialStage)
            {
                case 0:
                    new TutorialSelectLanguageDialog(session);
                    break;
                case 100:
                    new TutorialEnterNameDialog(session);
                    break;
                case 32000:
                    new TutorialEndDialog(session);
                    break;
            }
        }

        public static void StartNextStage(GameSession session)
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
            StartCurrentStage(session);
        }

        public static void CompleteTutorial(GameSession session)
        {
            session.profile.data.tutorialStage = -1;
            //TODO
        }

    }
}
