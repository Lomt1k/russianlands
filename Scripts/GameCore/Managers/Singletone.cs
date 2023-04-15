using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Managers
{
    public abstract class Singletone
    {
        public virtual void OnBotStarted(TelegramBot bot) { }
        public virtual void OnBotStopped(TelegramBot bot) { }
    }
}
