using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Scripts.TelegramBot.CallbackData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs
{
    public abstract class DialogBase
    {
        protected static SessionManager sessionManager => TelegramBot.instance.sessionManager;
        protected static MessageSender messageSender => TelegramBot.instance.messageSender;        

        private Dictionary<KeyboardButton, Func<Task>?> registeredButtons = new Dictionary<KeyboardButton, Func<Task>?>();
        private Dictionary<byte, DialogPanelBase> registeredPanels = new Dictionary<byte, DialogPanelBase>();

        public GameSession session { get; }
        public Tooltip? tooltip { get; private set; }
        protected int buttonsCount => registeredButtons.Count;
        protected Message? previousMessage { get; private set; }

        public DialogBase(GameSession _session)
        {
            session = _session;
            session.SetupActiveDialog(this);
        }

        protected void RegisterBackButton(Func<Task> callback)
        {
            RegisterButton($"{Emojis.elements[Element.Back]} {GameCore.Localizations.Localization.Get(session, "menu_item_back_button")}", callback);
        }

        protected void RegisterButton(string text, Func<Task>? callback)
        {
            registeredButtons.Add(text, callback);
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
                if (currentRow.Count == 2)
                {
                    rows.Add(currentRow);
                    currentRow = new List<KeyboardButton>();
                }
                currentRow.Add(button);
            }
            rows.Add(currentRow);

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

        protected async Task<Message> SendDialogMessage(StringBuilder sb, ReplyKeyboardMarkup? replyMarkup)
        {
            return await SendDialogMessage(sb.ToString(), replyMarkup);
        }

        protected async Task<Message> SendDialogMessage(string text, ReplyKeyboardMarkup? replyMarkup)
        {
            previousMessage = await messageSender.SendTextDialog(session.chatId, text, replyMarkup);
            return previousMessage;
        }

        protected async Task DeleteDialogMessage()
        {
            if (previousMessage != null)
            {
                await messageSender.DeleteMessage(session.chatId, previousMessage.MessageId);
                previousMessage = null;
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

                foreach (var button in buttonsToBlock)
                {
                    registeredButtons[button] = async () =>
                    {
                        session.tooltipController.SwitchToPrevious();
                        await Start();
                    };
                }

                // Модифицируем логику клика по нужной кнопке (если еще не модифицировали)
                if (!selectedButton.Text.Contains(Emojis.elements[Element.Warning]))
                {
                    var oldSelectedAction = registeredButtons[selectedButton];
                    bool needRemoveDialog = session.tooltipController.IfNextTooltipForPanelWithWaitingButtonClick();
                    var newStage = tooltip.stageAfterButtonClick;
                    Func<Task> newSelectedAction = async () =>
                    {
                        if (needRemoveDialog)
                        {
                            await DeleteDialogMessage();
                        }
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
                    registeredButtons[selectedButton] = () =>
                    {
                        tooltip = null;
                        return newSelectedAction();
                    };
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"{Emojis.elements[Element.Warning]} {Localization.Get(session, "dialog_tooltip_header")}");
            var hint = string.Format(Localization.Get(session, tooltip.localizationKey), selectedButton != null ? selectedButton.Text : string.Empty);
            sb.AppendLine(hint);

            if (selectedButton != null && !selectedButton.Text.Contains(Emojis.elements[Element.Warning]))
            {
                selectedButton.Text += ' ' + Emojis.elements[Element.Warning];
            }            

            return true;
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
