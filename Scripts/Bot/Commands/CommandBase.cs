using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services;

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
        protected static readonly MessageSender messageSender = Services.Get<MessageSender>();

        public abstract CommandGroup commandGroup { get; }

        public abstract Task Execute(GameSession session, string[] args);
    }
}
