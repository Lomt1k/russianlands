using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogPanelBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;
        public DialogBase dialog { get; }
        public GameSession session { get; }
        public byte panelId { get; }

        private Dictionary<int, InlineKeyboardButton> _registeredButtons = new Dictionary<int, InlineKeyboardButton>();
        private Dictionary<int, Func<Task>?> _registeredCallbacks = new Dictionary<int, Func<Task>?>();
        private Dictionary<int, Func<string?>> _registeredQueryAnswers = new Dictionary<int, Func<string?>>();
        private int _freeButtonId;

        protected int buttonsCount => _registeredButtons.Count;

        public DialogPanelBase(DialogBase _dialog, byte _panelId)
        {
            dialog = _dialog;
            session = _dialog.session;
            panelId = _panelId;
        }

        protected void RegisterButton(string text, Func<Task>? callback, Func<string?>? queryAnswer = null)
        {
            var callbackData = new DialogPanelButtonCallbackData()
            {
                panelId = panelId,
                buttonId = _freeButtonId,
            };
            var callbackDataJson = JsonConvert.SerializeObject(callbackData);

            _registeredButtons.Add(_freeButtonId, InlineKeyboardButton.WithCallbackData(text, callbackDataJson));
            _registeredCallbacks.Add(_freeButtonId, callback);
            _registeredQueryAnswers.Add(_freeButtonId, queryAnswer);
            _freeButtonId++;
        }

        protected void ClearButtons()
        {
            _registeredButtons.Clear();
            _registeredCallbacks.Clear();
            _registeredQueryAnswers.Clear();
            _freeButtonId = 0;
        }

        protected InlineKeyboardMarkup GetOneLineKeyboard()
        {
            return new InlineKeyboardMarkup(_registeredButtons.Values);
        }

        protected InlineKeyboardMarkup GetMultilineKeyboard()
        {
            var linesArray = new InlineKeyboardButton[_registeredButtons.Count][];
            int i = 0;
            foreach (var button in _registeredButtons.Values)
            {
                linesArray[i] = new InlineKeyboardButton[] { button };
                i++;
            }
            return new InlineKeyboardMarkup(linesArray);
        }

        protected InlineKeyboardMarkup GetKeyboardWithRowSizes(params int[] args)
        {
            var rows = new List<List<InlineKeyboardButton>>();
            var buttons = _registeredButtons.Values.ToList();

            int startIndex = 0;
            foreach (var count in args)
            {
                rows.Add(buttons.GetRange(startIndex, count));
                startIndex += count;
            }

            return new InlineKeyboardMarkup(rows);
        }

        public abstract Task SendAsync();
        public abstract void OnDialogClose();

        public virtual async Task HandleButtonPress(int buttonId, string queryId)
        {
            try
            {
                if (!_registeredCallbacks.TryGetValue(buttonId, out var callback))
                    return;

                var generateQueryFunc = _registeredQueryAnswers[buttonId];
                var query = generateQueryFunc != null ? generateQueryFunc() : null;

                if (callback != null)
                {
                    await callback();
                }
                await messageSender.AnswerQuery(queryId, query);
            }
            catch (Exception ex)
            {
                var error = $"Exception in DialogPanel (HandleButtonPress)\nSession for @{session.actualUser.Username} (userId {session.actualUser.Id})\n{ex}";
                Program.logger.Error(error + "\n");

                await messageSender.SendErrorMessage(session.chatId, ex.ToString());
                await messageSender.AnswerQuery(queryId);
            }
        }

    }
}
