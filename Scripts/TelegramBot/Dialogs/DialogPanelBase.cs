using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogPanelBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;
        public DialogBase dialog { get; }
        public GameSession session { get; }
        public Tooltip? tooltip { get; private set; }
        public byte panelId { get; }

        protected Message? lastMessage { get; set; } // необходимо присваивать, чтобы при выходе из диалога удалялся InlineKeyboard

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

        protected void RegisterBackButton(Func<Task> callback, Func<string?>? queryAnswer = null)
        {
            RegisterButton($"{Emojis.elements[Element.Back]} {GameCore.Localizations.Localization.Get(session, "menu_item_back_button")}", callback, queryAnswer);
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

        protected InlineKeyboardMarkup GetKeyboardWithFixedRowSize(int rowSize)
        {
            if (rowSize < 2 || buttonsCount < 3)
                return GetMultilineKeyboard();

            var buttons = _registeredButtons.Values;
            var rows = new List<List<InlineKeyboardButton>>();
            var currentRow = new List<InlineKeyboardButton>();

            foreach (var button in buttons)
            {
                if (currentRow.Count == 2)
                {
                    rows.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
                currentRow.Add(button);
            }
            rows.Add(currentRow);

            return new InlineKeyboardMarkup(rows);
        }

        public abstract Task SendAsync();

        protected async Task<Message> SendPanelMessage(StringBuilder sb, InlineKeyboardMarkup? inlineMarkup, bool asNewMessage = false)
        {
            return await SendPanelMessage(sb.ToString(), inlineMarkup, asNewMessage);
        }

        protected async Task<Message> SendPanelMessage(string text, InlineKeyboardMarkup? inlineMarkup, bool asNewMessage = false)
        {
            lastMessage = lastMessage == null || asNewMessage
                ? await messageSender.SendTextMessage(session.chatId, text, inlineMarkup)
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, inlineMarkup);

            return lastMessage;
        }

        public virtual void OnDialogClose() 
        {
            RemoveKeyboardFromLastMessage();
        }

        protected async Task RemoveKeyboardFromLastMessage()
        {
            ClearButtons();
            if (lastMessage?.ReplyMarkup != null)
            {
                await messageSender.EditMessageKeyboard(session.chatId, lastMessage.MessageId, null);
            }
        }

        public virtual async Task HandleButtonPress(int buttonId, string queryId)
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

        protected bool TryAppendTooltip(StringBuilder sb)
        {
            if (dialog.tooltip != null)
            {
                _registeredCallbacks.Clear();
                return false;
            }

            tooltip = session.tooltipController.TryGetTooltip(this);
            if (tooltip == null)
                return false;

            int? selectedButton = null;
            if (tooltip.buttonId > -1 && tooltip.buttonId < buttonsCount)
            {
                var buttonsList = _registeredButtons.Keys.ToList();
                selectedButton = buttonsList[tooltip.buttonId];

                var selectedButtonText = _registeredButtons[selectedButton.Value].Text;
                var hintBlock = string.Format(Localization.Get(session, tooltip.localizationKey), selectedButtonText).RemoveHtmlTags();

                var buttonsToBlock = new List<int>();
                foreach (var button in _registeredButtons)
                {
                    if (button.Key != selectedButton)
                    {
                        buttonsToBlock.Add(button.Key);
                    }
                }

                foreach (var button in buttonsToBlock)
                {
                    _registeredCallbacks[button] = null;
                    _registeredQueryAnswers[button] = () =>
                    {
                        return hintBlock;
                    };
                }
            }

            // Модифицируем логику клика по нужной кнопке
            if (selectedButton.HasValue)
            {
                var oldSelectedAction = _registeredCallbacks[selectedButton.Value];
                var newStage = tooltip.stageAfterButtonClick;
                Func<Task> newSelectedAction = async () =>
                {
                    if (oldSelectedAction != null)
                    {
                        await oldSelectedAction();
                    }
                    if (newStage > -1)
                    {
                        var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
                        if (focusedQuest != null)
                        {
                            var quest = GameCore.Quests.QuestsHolder.GetQuest(focusedQuest.Value);
                            await quest.SetStage(session, newStage);
                        }
                    }
                };
                _registeredCallbacks[selectedButton.Value] = () =>
                {
                    tooltip = null;
                    return newSelectedAction();
                };
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"{Emojis.elements[Element.Warning]} {Localization.Get(session, "dialog_tooltip_header")}");
            var hint = string.Format(
                Localization.Get(session, tooltip.localizationKey), selectedButton.HasValue ? _registeredButtons[selectedButton.Value].Text : string.Empty);
            sb.AppendLine(hint);

            if (selectedButton.HasValue)
            {
                var button = _registeredButtons[selectedButton.Value];
                if (!button.Text.Contains(Emojis.elements[Element.Warning]))
                {
                    button.Text += ' ' + Emojis.elements[Element.Warning];
                }
            }

            return true;
        }

    }
}
