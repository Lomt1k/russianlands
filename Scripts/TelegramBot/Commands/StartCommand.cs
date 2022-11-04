using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    // ввод команды /start при запущенной(!) сессии
    public class StartCommand : CommandBase
    {
        public override bool isAdminCommand => false;

        public override async Task Execute(GameSession session, string[] args)
        {
            var currentDialog = session.currentDialog;
            if (currentDialog != null)
            {
                await currentDialog.TryResendDialogWithAntiFloodDelay();
            }
        }
    }
}
