using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Services
{
    public abstract class Service
    {
        public virtual void OnBotStarted(TelegramBot bot) { }
        public virtual void OnBotStopped(TelegramBot bot) { }
    }
}
