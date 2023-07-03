using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Commands;

public enum CommandGroup
{
    ForAll,
    Cheat,
    Admin
}

public abstract class CommandBase
{
    protected static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();

    public abstract CommandGroup commandGroup { get; }

    public abstract Task Execute(GameSession session, string[] args);
}
