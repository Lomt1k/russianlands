using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;

namespace MarkOne.Scripts.Bot.Commands;

// ввод команды /start при запущенной(!) сессии
public class StartCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.ForAll;

    public override async Task Execute(GameSession session, string[] args)
    {
        var currentDialog = session.currentDialog;
        if (currentDialog != null)
        {
            await currentDialog.TryResendDialogWithAntiFloodDelay();
        }
    }
}
