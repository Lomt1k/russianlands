using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands
{
    public enum CommandGroup
    {
        ForAll,
        Cheat,
        Admin
    }

    public abstract class CommandBase
    {
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;

        public abstract CommandGroup commandGroup { get; }

        public abstract Task Execute(GameSession session, string[] args);
    }
}
