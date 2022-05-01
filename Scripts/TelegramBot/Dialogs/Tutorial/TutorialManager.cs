using Telegram.Bot.Types;
using TextGameRPG.Scripts.GameCore.Profiles;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Tutorial
{
    public static class TutorialManager
    {
        public static void StartCurrentStage(User actualUser, Profile profile)
        {
            switch (profile.data.tutorialStage)
            {
                case 0:
                    new TutorialEnterNameDialog(actualUser).Init(profile.data.telegram_id);
                    break;
            }


        }
    }
}
