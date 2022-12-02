using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands
{
    public abstract class CommandBase
    {
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;

        public abstract bool isAdminCommand { get; }

        public abstract Task Execute(GameSession session, string[] args);
    }
}
