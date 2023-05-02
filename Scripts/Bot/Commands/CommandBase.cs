using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services;

namespace MarkOne.Scripts.Bot.Commands;

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
