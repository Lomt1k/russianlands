using System.Threading.Tasks;
using Telegram.Bot.Types;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;

        protected long chatId { get; private set; }
        protected GameSession? session { get; private set; }

        public DialogBase? Init(long _chatId)
        {
            chatId = _chatId;
            session = sessionManager.GetSessionIfExists(_chatId);
            if (session == null)
                return null;

            session.SetupActiveDialog(this);
            Start();
            return this;
        }

        protected abstract void Start();

        public abstract void HandleMessage(User actualUser, Message message);


    }
}
