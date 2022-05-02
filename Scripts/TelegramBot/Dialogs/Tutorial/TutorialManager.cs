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
                    new TutorialSelectLanguage(session);
                    break;
                case 100:
                    new TutorialEnterNameDialog(session);
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
            }

            session.profile.data.tutorialStage = stage;
            StartCurrentStage(session);
        }

    }
}
