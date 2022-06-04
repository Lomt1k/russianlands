using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;
        public GameSession session { get; }

        private Dictionary<KeyboardButton, Func<Task>?> registeredButtons = new Dictionary<KeyboardButton, Func<Task>?>();
        private Dictionary<byte, DialogPanelBase> registeredPanels = new Dictionary<byte, DialogPanelBase>();

        public DialogBase(GameSession _session)
        {
            session = _session;
            session.SetupActiveDialog(this);
        }

        protected void RegisterButton(string text, Func<Task>? callback)
        {
            registeredButtons.Add(text, callback);
        }

        protected void RegisterPanel(DialogPanelBase dialogPanel)
        {
            registeredPanels.Add(dialogPanel.panelId, dialogPanel);
        }

        protected ReplyKeyboardMarkup GetOneLineKeyboard()
        {
            return new ReplyKeyboardMarkup(registeredButtons.Keys);
        }

        protected ReplyKeyboardMarkup GetMultilineKeyboard()
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

        protected ReplyKeyboardMarkup GetKeyboardWithRowSizes(params int[] args)
        {
            var rows = new List<List<KeyboardButton>>();
            var buttons = registeredButtons.Keys.ToList();

            int startIndex = 0;
            foreach (var count in args)
            {
                rows.Add(buttons.GetRange(startIndex, count));
                startIndex += count;
            }

            return new ReplyKeyboardMarkup(rows);
        }

        protected async Task SendPanelsAsync()
        {
            foreach (var panel in registeredPanels.Values)
            {
                await panel.SendAsync();
            }
        }

        public abstract Task Start();

        public virtual async Task HandleMessage(Message message)
        {
            var text = message.Text;
            if (text == null)
                return;

            Func<Task>? callback = null;
            foreach (var button in registeredButtons.Keys)
            {
                if (button.Text.Equals(text))
                {
                    registeredButtons.TryGetValue(button, out callback);
                    break;
                }
            }

            if (callback != null)
            {
                await Task.Run(callback);
            }
        }

        public virtual async Task HandleCallbackQuery(string queryId, DialogPanelButtonCallbackData callback)
        {
            if (registeredPanels.TryGetValue(callback.panelId, out var panel))
            {
                await panel.HandleButtonPress(callback.buttonId, queryId);
            }
        }

        public virtual void OnClose()
        {
            foreach (var panel in registeredPanels.Values)
            {
                panel.OnDialogClose();
            }
        }


    }
}
