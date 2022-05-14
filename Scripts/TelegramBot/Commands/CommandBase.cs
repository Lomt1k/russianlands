using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public abstract class CommandBase
    {
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;

        public abstract void Execute(GameSession session, string[] args);
    }
}
