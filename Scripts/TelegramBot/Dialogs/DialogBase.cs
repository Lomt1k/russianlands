using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;
        protected GameSession session { get; }

        private Dictionary<KeyboardButton, Action> registeredButtons = new Dictionary<KeyboardButton, Action>();

        public DialogBase(GameSession _session)
        {
            session = _session;
            session.SetupActiveDialog(this);
            Start();
        }

        protected void RegisterButton(string text, Action callback)
        {
            registeredButtons.Add(text, callback);
        }

        protected ReplyKeyboardMarkup GetKeyboard()
        {
            return new ReplyKeyboardMarkup(registeredButtons.Keys);
        }

        protected ReplyKeyboardMarkup GetKeyboardMultiline()
        {
            var linesArray = new KeyboardButton[registeredButtons.Count][];
            int i = 0;
            foreach (var button in registeredButtons.Keys)
            {
                linesArray[i] = new KeyboardButton[] { button };
                i++;
            }
            return new ReplyKeyboardMarkup(linesArray);
        }

        protected abstract void Start();

        public virtual void HandleMessage(Message message)
        {
            var text = message.Text;
            if (text != null)
            {
                foreach (var button in registeredButtons.Keys)
                {
                    if (button.Text.Equals(text))
                    {
                        var buttonCallback = registeredButtons[button];
                        buttonCallback();
                    }
                }
            }
        }


    }
}
