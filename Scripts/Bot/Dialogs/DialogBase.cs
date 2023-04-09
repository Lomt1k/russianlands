using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Managers;
using TextGameRPG.Scripts.Bot.Dialogs.Town;

namespace TextGameRPG.Scripts.Bot.Dialogs
{
    public abstract class DialogBase
    {
        private static readonly NotificationsManager notificationsManager = Singletones.Get<NotificationsManager>();

        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;        

        private Dictionary<KeyboardButton, Func<Task>?> _registeredButtons = new Dictionary<KeyboardButton, Func<Task>?>();
        private Func<Task<Message?>>? _resendLastMessageFunc;

        public GameSession session { get; }
        public Tooltip? tooltip { get; private set; }
        protected int buttonsCount => _registeredButtons.Count;
        protected Message? lastMessage { get; private set; }

        public DialogBase(GameSession _session)
        {
            session = _session;
            session.SetupActiveDialog(this);
        }

        protected void RegisterButton(string text, Func<Task>? callback)
        {
            _registeredButtons.Add(text, callback);
        }

        protected void RegisterBackButton(Func<Task> callback)
        {
            RegisterButton(Emojis.ElementBack + Localization.Get(session, "menu_item_back_button"), callback);
        }

        protected void RegisterBackButton(string text, Func<Task> callback)
        {
            RegisterButton(Emojis.ElementBack + text, callback);
        }

        protected void RegisterDoubleBackButton(string text, Func<Task> callback)
        {
            RegisterButton(Emojis.ElementBack + text, callback);
        }

        protected void RegisterTownButton(bool isDoubleBack)
        {
            var emojiBack = isDoubleBack ? Emojis.ElementDoubleBack : Emojis.ElementBack;
            RegisterButton(emojiBack + Localization.Get(session, "menu_item_town") + Emojis.ButtonTown, 
                () => notificationsManager.GetNotificationsAndEntryTown(session, TownEntryReason.BackFromInnerDialog));
        }        

        protected void ClearButtons()
        {
            _registeredButtons.Clear();
        }

        protected ReplyKeyboardMarkup GetOneLineKeyboard()
        {
            return new ReplyKeyboardMarkup(_registeredButtons.Keys);
        }

        protected ReplyKeyboardMarkup GetMultilineKeyboard()
        {
            var linesArray = new KeyboardButton[_registeredButtons.Count][];
            int i = 0;
            foreach (var button in _registeredButtons.Keys)
            {
                linesArray[i] = new KeyboardButton[] { button };
                i++;
            }
            return new ReplyKeyboardMarkup(linesArray);
        }

        protected ReplyKeyboardMarkup GetKeyboardWithRowSizes(params int[] args)
        {
            var rows = new List<List<KeyboardButton>>();
            var buttons = _registeredButtons.Keys.ToList();

            int startIndex = 0;
            foreach (var count in args)
            {
                rows.Add(buttons.GetRange(startIndex, count));
                startIndex += count;
            }

            return new ReplyKeyboardMarkup(rows);
        }

        protected ReplyKeyboardMarkup GetKeyboardWithFixedRowSize(int rowSize)
        {
            if (rowSize < 2 || buttonsCount < 3)
                return GetMultilineKeyboard();
            
            var buttons = _registeredButtons.Keys;
            var rows = new List<List<KeyboardButton>>();
            var currentRow = new List<KeyboardButton>();

            foreach (var button in buttons)
            {
                if (currentRow.Count == rowSize)
                {
                    rows.Add(currentRow);
                    currentRow = new List<KeyboardButton>();
                }
                currentRow.Add(button);
            }
            rows.Add(currentRow);

            return new ReplyKeyboardMarkup(rows);
        }

        protected ReplyKeyboardMarkup GetMultilineKeyboardWithDoubleBack()
        {
            var buttons = _registeredButtons.Keys.ToArray();
            var rowsCount = buttons.Length - 1;
            var rows = new List<List<KeyboardButton>>(rowsCount);
            
            var lastRow = new List<KeyboardButton>
            {
                buttons[buttonsCount - 2],
                buttons[buttonsCount - 1]
            };

            for (int i = 0; i < buttonsCount - 2; i++)
            {
                var row = new List<KeyboardButton> { buttons[i] };
                rows.Add(row);
            }
            rows.Add(lastRow);
            
            return new ReplyKeyboardMarkup(rows);
        }

        public abstract Task Start();

        protected async Task<Message?> SendDialogMessage(StringBuilder sb, ReplyKeyboardMarkup? replyMarkup)
        {
            return await SendDialogMessage(sb.ToString(), replyMarkup).FastAwait();
        }

        protected async Task<Message?> SendDialogMessage(string text, ReplyKeyboardMarkup? replyMarkup)
        {
            _resendLastMessageFunc = async() => await messageSender.SendTextDialog(session.chatId, text, replyMarkup).FastAwait();
            lastMessage = await _resendLastMessageFunc().FastAwait();
            return lastMessage;
        }

        public async Task TryResendDialogWithAntiFloodDelay()
        {
            if (lastMessage == null)
                return;

            var secondsFromPreviousSent = (DateTime.UtcNow - lastMessage.Date).TotalSeconds;
            if (lastMessage == null || secondsFromPreviousSent < 3)
                return;

            await TryResendDialog().FastAwait();
        }

        public virtual async Task TryResendDialog()
        {
            if (_resendLastMessageFunc == null)
                return;

            lastMessage = await _resendLastMessageFunc().FastAwait();
        }

        public virtual async Task HandleMessage(Message message)
        {
            var text = message.Text;
            if (text == null)
                return;

            Func<Task>? callback = null;
            foreach (var button in _registeredButtons.Keys)
            {
                if (button.Text.Equals(text))
                {
                    _registeredButtons.TryGetValue(button, out callback);
                    break;
                }
            }

            if (callback != null)
            {
                await Task.Run(callback).FastAwait();
            }
            else
            {
                // пришел какой-то другой ответ от игрока (не одна из кнопок). Возможно у него пропала клавиатура или весь чат, надо переотправить диалог
                await TryResendDialogWithAntiFloodDelay().FastAwait();
            }
        }

        protected bool TryAppendTooltip(StringBuilder sb, Tooltip? _tooltip = null)
        {
            tooltip = _tooltip ?? session.tooltipController.TryGetTooltip(this);
            if (tooltip == null)
                return false;

            KeyboardButton? selectedButton = null;
            if (tooltip.buttonId > -1 && tooltip.buttonId < buttonsCount)
            {
                var buttonsList = _registeredButtons.Keys.ToList();
                selectedButton = buttonsList[tooltip.buttonId];

                var buttonsToBlock = new List<KeyboardButton>();
                foreach (var button in _registeredButtons)
                {
                    if (button.Key != selectedButton)
                    {
                        buttonsToBlock.Add(button.Key);
                    }
                }
                BlockButtons(buttonsToBlock);

                // Модифицируем логику клика по нужной кнопке (если еще не модифицировали)
                if (!selectedButton.Text.Contains(Emojis.ElementWarning.ToString()))
                {
                    var oldSelectedAction = _registeredButtons[selectedButton];
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
                    _registeredButtons[selectedButton] = () =>
                    {
                        tooltip = null;
                        return newSelectedAction();
                    };
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Emojis.ElementWarning + Localization.Get(session, "dialog_tooltip_header"));
            var hint = Localization.Get(session, tooltip.localizationKey, selectedButton?.Text ?? string.Empty);
            sb.AppendLine(hint);

            if (selectedButton != null && !selectedButton.Text.Contains(Emojis.ElementWarning.ToString()))
            {
                selectedButton.Text += Emojis.ElementWarning;
            }            

            return true;
        }

        public void BlockAllButtons()
        {
            var allButtons = _registeredButtons.Keys.ToArray();
            BlockButtons(allButtons);
        }

        private void BlockButtons(IEnumerable<KeyboardButton> buttonsToBlock)
        {
            foreach (var button in buttonsToBlock)
            {
                _registeredButtons[button] = () => TryResendDialog();
            }
        }

        public virtual void OnClose()
        {
            //ignored
        }

    }
}
