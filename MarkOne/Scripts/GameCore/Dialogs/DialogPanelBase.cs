using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot.CallbackData;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using FastTelegramBot.DataTypes;
using FastTelegramBot.DataTypes.Keyboards;
using FastTelegramBot.DataTypes.InputFiles;

namespace MarkOne.Scripts.GameCore.Dialogs;

public abstract class DialogPanelBase
{
    protected static readonly MessageSender messageSender = ServiceLocator.Get<MessageSender>();
    protected static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();
    protected static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    public DialogWithPanel dialog { get; }
    public GameSession session { get; }
    public Tooltip? tooltip { get; private set; }

    private Func<Task<MessageId>>? _resendLastMessageFunc;
    private MessageId? _lastMessageId;
    private int _aliveQueryHash;

    private readonly Dictionary<int, InlineKeyboardButton> _registeredButtons = new Dictionary<int, InlineKeyboardButton>();
    private readonly Dictionary<int, Func<Task>?> _registeredCallbacks = new Dictionary<int, Func<Task>?>();
    private readonly Dictionary<int, Func<string?>> _registeredQueryAnswers = new Dictionary<int, Func<string?>>();
    private int _freeButtonId;

    protected int buttonsCount => _registeredButtons.Count;

    public DialogPanelBase(DialogWithPanel _dialog)
    {
        dialog = _dialog;
        session = _dialog.session;
        _aliveQueryHash = GetHashCode();
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
        RegisterButton(Emojis.ElementDoubleBack + text, callback);
    }

    protected void RegisterButton(string text, Func<Task>? callback, Func<string?>? queryAnswer = null)
    {
        var callbackData = new DialogPanelButtonCallbackData()
        {
            aliveQueryHash = _aliveQueryHash,
            buttonId = _freeButtonId,
        };
        var callbackDataJson = JsonConvert.SerializeObject(callbackData);

        _registeredButtons.Add(_freeButtonId, InlineKeyboardButton.WithCallbackData(text, callbackDataJson));
        _registeredCallbacks.Add(_freeButtonId, callback);
        _registeredQueryAnswers.Add(_freeButtonId, queryAnswer);
        _freeButtonId++;
    }

    protected void RegisterLinkButton(string text, string url)
    {
        _registeredButtons.Add(_freeButtonId, InlineKeyboardButton.WithUrl(text, url));
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
        var linesArray = new List<List<InlineKeyboardButton>>();
        foreach (var button in _registeredButtons.Values)
        {
            linesArray.Add( new List<InlineKeyboardButton> { button } );
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

        for (var i = 0; i < buttonsCount - 2; i++)
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

        var startIndex = 0;
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

    protected async Task<MessageId> SendPanelMessage(StringBuilder sb, InlineKeyboardMarkup? inlineMarkup)
    {
        return await SendPanelMessage(sb.ToString(), inlineMarkup).FastAwait();
    }

    protected async Task<MessageId> SendPanelMessage(string text, InlineKeyboardMarkup? inlineMarkup)
    {

        _resendLastMessageFunc = async () =>
        {
            RefreshAliveQueryHashInButtons(inlineMarkup);
            return await messageSender.SendTextMessage(session.chatId, text, inlineMarkup, disableWebPagePreview: true, cancellationToken: session.cancellationToken).FastAwait();
        };

        if (_lastMessageId is null)
        {
            _lastMessageId = await _resendLastMessageFunc().FastAwait();
        }
        else
        {
            await messageSender.EditTextMessage(session.chatId, _lastMessageId.Value, text, inlineMarkup, disableWebPagePreview: true, cancellationToken: session.cancellationToken).FastAwait();
        }
        return _lastMessageId.Value;
    }

    protected async Task<MessageId> SendPanelPhotoMessage(InputFile photo, StringBuilder sb, InlineKeyboardMarkup? inlineMarkup)
    {
        return await SendPanelPhotoMessage(photo, sb.ToString(), inlineMarkup).FastAwait();
    }

    protected async Task<MessageId> SendPanelPhotoMessage(InputFile photo, string text, InlineKeyboardMarkup? inlineMarkup)
    {
        RefreshAliveQueryHashInButtons(inlineMarkup);
        _resendLastMessageFunc = async () =>
        {
            RefreshAliveQueryHashInButtons(inlineMarkup);
            return await messageSender.SendPhotoMessage(session.chatId, photo, text, inlineMarkup, cancellationToken: session.cancellationToken).FastAwait();
        };

        if (_lastMessageId is null)
        {
            _lastMessageId = await _resendLastMessageFunc().FastAwait();
        }
        else
        {
            await messageSender.EditPhotoMessageCaption(session.chatId, _lastMessageId.Value, text, inlineMarkup, disableWebPagePreview: true, cancellationToken: session.cancellationToken).FastAwait();
        }
        return _lastMessageId.Value;
    }

    protected async Task EditPhotoMessageCaption(string text, InlineKeyboardMarkup? inlineMarkup)
    {
        await messageSender.EditPhotoMessageCaption(session.chatId, _lastMessageId.Value, text, inlineMarkup, disableWebPagePreview: true, cancellationToken: session.cancellationToken).FastAwait();
    }

    private void RefreshAliveQueryHashInButtons(InlineKeyboardMarkup? inlineMarkup)
    {
        if (inlineMarkup is null)
        {
            return;
        }

        _aliveQueryHash = _lastMessageId?.Id.GetHashCode() ?? GetHashCode();
        foreach (var row in inlineMarkup.InlineKeyboard)
        {
            foreach (var button in row)
            {
                if (!string.IsNullOrEmpty(button.CallbackData))
                {
                    var callback = JsonConvert.DeserializeObject<DialogPanelButtonCallbackData>(button.CallbackData);
                    if (callback is not null)
                    {
                        callback.aliveQueryHash = _aliveQueryHash;
                        button.CallbackData = JsonConvert.SerializeObject(callback);
                    }
                }
            }
        }
    }

    public virtual void OnDialogClose()
    {
        // ignored
    }

    public virtual async Task HandleButtonPress(DialogPanelButtonCallbackData callbackData, string queryId)
    {
        if (callbackData.aliveQueryHash != _aliveQueryHash)
        {
            await messageSender.AnswerQuery(queryId, Localization.Get(session, "invalid_query_answer"), session.cancellationToken).FastAwait();
            return;
        }

        var buttonId = callbackData.buttonId;
        if (!_registeredCallbacks.TryGetValue(buttonId, out var callback))
        {
            return;
        }

        if (BotController.config.logSettings.logUpdates)
        {
            Program.logger.Info($"UPDATE :: {session.actualUser}: {_registeredButtons[buttonId].Text}");
        }

        var generateQueryFunc = _registeredQueryAnswers[buttonId];
        var query = generateQueryFunc != null ? generateQueryFunc() : null;

        if (callback != null)
        {
            await callback().FastAwait();
        }
        await messageSender.AnswerQuery(queryId, query, session.cancellationToken).FastAwait();
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
                        var quest = gameDataHolder.quests[focusedQuest.Value];
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
        if (_lastMessageId is null || _resendLastMessageFunc is null)
        {
            return;
        }

        _aliveQueryHash = _lastMessageId.Value.Id.GetHashCode();
        _lastMessageId = await _resendLastMessageFunc().FastAwait();
    }

}
