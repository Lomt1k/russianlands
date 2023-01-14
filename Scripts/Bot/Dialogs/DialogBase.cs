using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.Bot.CallbackData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs
{
    public abstract class DialogBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;        

        private Dictionary<KeyboardButton, Func<Task>?> registeredButtons = new Dictionary<KeyboardButton, Func<Task>?>();
        private Dictionary<byte, DialogPanelBase> registeredPanels = new Dictionary<byte, DialogPanelBase>();
        private Func<Task<Message?>>? resendLastMessageFunc;

        public GameSession session { get; }
        public Tooltip? tooltip { get; private set; }
        protected int buttonsCount => registeredButtons.Count;
        protected Message? lastMessage { get; private set; }

        public DialogBase(GameSession _session)
        {
            session = _session;
            session.SetupActiveDialog(this);
        }

        protected void RegisterButton(string text, Func<Task>? callback)
        {
            registeredButtons.Add(text, callback);
        }

        protected void RegisterBackButton(Func<Task> callback)
        {
            RegisterButton(Emojis.ElementSmallBlack + Localization.Get(session, "menu_item_back_button"), callback);
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
                () => new Town.TownDialog(session, Town.TownEntryReason.BackFromInnerDialog).Start());
        }        

        protected void ClearButtons()
        {
            registeredButtons.Clear();
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

        protected ReplyKeyboardMarkup GetKeyboardWithFixedRowSize(int rowSize)
        {
            if (rowSize < 2 || buttonsCount < 3)
                return GetMultilineKeyboard();
            
            var buttons = registeredButtons.Keys;
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
            switch (buttonsCount)
            {
                case 3: return GetKeyboardWithRowSizes(1, 2);
                case 4: return GetKeyboardWithRowSizes(1, 1, 2);
                case 5: return GetKeyboardWithRowSizes(1, 1, 1, 2);
                default: return GetKeyboardWithRowSizes(2);
            }
        }

        protected async Task SendPanelsAsync()
        {
            foreach (var panel in registeredPanels.Values)
            {
                await panel.SendAsync()
                    .ConfigureAwait(false);
            }
        }

        public abstract Task Start();

        protected async Task<Message?> SendDialogMessage(StringBuilder sb, ReplyKeyboardMarkup? replyMarkup)
        {
            return await SendDialogMessage(sb.ToString(), replyMarkup)
                .ConfigureAwait(false);
        }

        protected async Task<Message?> SendDialogMessage(string text, ReplyKeyboardMarkup? replyMarkup)
        {
            resendLastMessageFunc = async() => await messageSender.SendTextDialog(session.chatId, text, replyMarkup)
                .ConfigureAwait(false);
            lastMessage = await resendLastMessageFunc()
                .ConfigureAwait(false);
            return lastMessage;
        }

        public async Task TryResendDialogWithAntiFloodDelay()
        {
            if (lastMessage == null)
                return;

            var secondsFromPreviousSent = (DateTime.UtcNow - lastMessage.Date).TotalSeconds;
            if (lastMessage == null || secondsFromPreviousSent < 3)
                return;

            await TryResendDialog()
                .ConfigureAwait(false);
        }

        public virtual async Task TryResendDialog()
        {
            if (resendLastMessageFunc == null)
                return;

            lastMessage = await resendLastMessageFunc()
                .ConfigureAwait(false);
            foreach (var panel in registeredPanels.Values)
            {
                await panel.ResendLastMessageAsNew()
                    .ConfigureAwait(false);
            }
        }

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
                await Task.Run(callback).ConfigureAwait(false);
            }
            else
            {
                // пришел какой-то другой ответ от игрока (не одна из кнопок). Возможно у него пропала клавиатура или весь чат, надо переотправить диалог
                await TryResendDialogWithAntiFloodDelay()
                    .ConfigureAwait(false);
            }
        }

        public virtual async Task HandleCallbackQuery(string queryId, DialogPanelButtonCallbackData callback)
        {
            if (registeredPanels.TryGetValue(callback.panelId, out var panel))
            {
                await panel.HandleButtonPress(callback.buttonId, queryId)
                    .ConfigureAwait(false);
            }
        }

        protected bool TryAppendTooltip(StringBuilder sb)
        {
            tooltip = session.tooltipController.TryGetTooltip(this);
            if (tooltip == null)
                return false;

            KeyboardButton? selectedButton = null;
            if (tooltip.buttonId > -1 && tooltip.buttonId < buttonsCount)
            {
                var buttonsList = registeredButtons.Keys.ToList();
                selectedButton = buttonsList[tooltip.buttonId];

                var buttonsToBlock = new List<KeyboardButton>();
                foreach (var button in registeredButtons)
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
                    var oldSelectedAction = registeredButtons[selectedButton];
                    var newStage = tooltip.stageAfterButtonClick;
                    Func<Task> newSelectedAction = async () =>
                    {
                        if (oldSelectedAction != null)
                        {
                            await oldSelectedAction().ConfigureAwait(false);
                        }
                        if (newStage > -1)
                        {
                            var focusedQuest = session.profile.dynamicData.quests.GetFocusedQuest();
                            if (focusedQuest != null)
                            {
                                var quest = GameCore.Quests.QuestsHolder.GetQuest(focusedQuest.Value);
                                await quest.SetStage(session, newStage)
                                    .ConfigureAwait(false);
                            }
                        }
                    };
                    registeredButtons[selectedButton] = () =>
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
            var allButtons = registeredButtons.Keys.ToArray();
            BlockButtons(allButtons);
        }

        private void BlockButtons(IEnumerable<KeyboardButton> buttonsToBlock)
        {
            foreach (var button in buttonsToBlock)
            {
                registeredButtons[button] = () => TryResendDialog();
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
