using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.Sessions;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Text;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.Scripts.Bot.Dialogs
{
    public abstract class DialogPanelBase
    {
        protected static readonly MessageSender messageSender = Services.Get<MessageSender>();
        public DialogWithPanel dialog { get; }
        public GameSession session { get; }
        public Tooltip? tooltip { get; private set; }

        protected Message? lastMessage { get; set; } // необходимо присваивать, чтобы при выходе из диалога удалялся InlineKeyboard

        private Dictionary<int, InlineKeyboardButton> _registeredButtons = new Dictionary<int, InlineKeyboardButton>();
        private Dictionary<int, Func<Task>?> _registeredCallbacks = new Dictionary<int, Func<Task>?>();
        private Dictionary<int, Func<string?>> _registeredQueryAnswers = new Dictionary<int, Func<string?>>();
        private int _freeButtonId;

        protected int buttonsCount => _registeredButtons.Count;

        public DialogPanelBase(DialogWithPanel _dialog)
        {
            dialog = _dialog;
            session = _dialog.session;
        }

        protected void RegisterBackButton(Func<Task> callback, Func<string?>? queryAnswer = null)
        {
            RegisterButton(Emojis.ElementBack + Localization.Get(session, "menu_item_back_button"), callback, queryAnswer);
        }

        protected void RegisterBackButton(string text, Func<Task> callback)
        {
            RegisterButton(Emojis.ElementBack + text, callback);
        }

        protected void RegisterDoubleBackButton(string text, Func<Task> callback)
        {
            RegisterButton(Emojis.ElementBack + text, callback);
        }

        protected void RegisterButton(string text, Func<Task>? callback, Func<string?>? queryAnswer = null)
        {
            var callbackData = new DialogPanelButtonCallbackData()
            {
                sessionTime = session.startTime.Ticks,
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

        protected InlineKeyboardMarkup GetMultilineKeyboardWithDoubleBack()
        {
            var buttons = _registeredButtons.Values.ToArray();
            var rowsCount = buttons.Length - 1;
            var rows = new List<List<InlineKeyboardButton>>(rowsCount);

            var lastRow = new List<InlineKeyboardButton>
            {
                buttons[buttonsCount - 2],
                buttons[buttonsCount - 1]
            };

            for (int i = 0; i < buttonsCount - 2; i++)
            {
                var row = new List<InlineKeyboardButton> { buttons[i] };
                rows.Add(row);
            }
            rows.Add(lastRow);

            return new InlineKeyboardMarkup(rows);
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
                if (currentRow.Count == rowSize)
                {
                    rows.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
                currentRow.Add(button);
            }
            rows.Add(currentRow);

            return new InlineKeyboardMarkup(rows);
        }

        public abstract Task Start();

        protected async Task<Message> SendPanelMessage(StringBuilder sb, InlineKeyboardMarkup? inlineMarkup, bool asNewMessage = false)
        {
            return await SendPanelMessage(sb.ToString(), inlineMarkup, asNewMessage).FastAwait();
        }

        protected async Task<Message> SendPanelMessage(string text, InlineKeyboardMarkup? inlineMarkup, bool asNewMessage = false)
        {
            lastMessage = lastMessage == null || asNewMessage
                ? await messageSender.SendTextMessage(session.chatId, text, inlineMarkup).FastAwait()
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, inlineMarkup).FastAwait();

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
                await messageSender.EditMessageKeyboard(session.chatId, lastMessage.MessageId, null).FastAwait();
            }
        }

        public virtual async Task HandleButtonPress(int buttonId, string queryId)
        {
            if (!_registeredCallbacks.TryGetValue(buttonId, out var callback))
                return;

            if (BotController.config.logUserInput)
            {
                Program.logger.Info($"Message from {session.actualUser}: {_registeredButtons[buttonId].Text}");
            }            

            var generateQueryFunc = _registeredQueryAnswers[buttonId];
            var query = generateQueryFunc != null ? generateQueryFunc() : null;

            if (callback != null)
            {
                await callback().FastAwait();
            }
            await messageSender.AnswerQuery(session.chatId, queryId, query).FastAwait();
        }

        protected bool TryAppendTooltip(StringBuilder sb, Tooltip? _tooltip = null)
        {
            if (dialog.tooltip != null)
            {
                _registeredCallbacks.Clear();
                return false;
            }

            tooltip = _tooltip ?? session.tooltipController.TryGetTooltip(this);
            if (tooltip == null)
                return false;

            dialog.BlockAllButtons();
            int? selectedButton = null;
            if (tooltip.buttonId > -1 && tooltip.buttonId < buttonsCount)
            {
                var buttonsList = _registeredButtons.Keys.ToList();
                selectedButton = buttonsList[tooltip.buttonId];

                var selectedButtonText = _registeredButtons[selectedButton.Value].Text;
                var hintBlock = Localization.Get(session, tooltip.localizationKey, selectedButtonText).RemoveHtmlTags();

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
                        await oldSelectedAction().FastAwait();
                    }
                    if (newStage > -1)
                    {
                        var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
                        if (focusedQuest != null)
                        {
                            var quest = GameCore.Quests.QuestsHolder.GetQuest(focusedQuest.Value);
                            await quest.SetStage(session, newStage).FastAwait();
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
            sb.AppendLine(Emojis.ElementWarning + Localization.Get(session, "dialog_tooltip_header"));
            var hint = Localization.Get(session, tooltip.localizationKey, 
                selectedButton.HasValue ? _registeredButtons[selectedButton.Value].Text : string.Empty);
            sb.AppendLine(hint);

            if (selectedButton.HasValue)
            {
                var button = _registeredButtons[selectedButton.Value];
                if (!button.Text.Contains(Emojis.ElementWarning.ToString()))
                {
                    button.Text += Emojis.ElementWarning;
                }
            }

            return true;
        }

        public async Task ResendLastMessageAsNew()
        {
            if (lastMessage == null)
                return;

            await messageSender.DeleteMessage(lastMessage.Chat.Id, lastMessage.MessageId).FastAwait();
            lastMessage = await messageSender.SendTextMessage(session.chatId, lastMessage.Text, lastMessage.ReplyMarkup).FastAwait();
        }

    }
}
