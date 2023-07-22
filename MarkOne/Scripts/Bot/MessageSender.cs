using System;
using System.Threading;
using System.Threading.Tasks;
using FastTelegramBot;
using FastTelegramBot.DataTypes;
using FastTelegramBot.DataTypes.InputFiles;
using FastTelegramBot.DataTypes.Keyboards;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.Sending;

namespace MarkOne.Scripts.Bot;

public class MessageSender : Service
{
    private static readonly MessageSequencer sequencer = ServiceLocator.Get<MessageSequencer>();

    private TelegramBotClient _botClient;
    private int _maxDelayForSendStickers;

    public override Task OnBotStarted()
    {
        _botClient = BotController.botClient;
        _maxDelayForSendStickers = BotController.config.sendingLimits.dontSendStickerIfDelayInSeconds * 1_000;
        return Task.CompletedTask;
    }

    public async Task<MessageId> SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool silent = false, bool disableWebPagePreview = false, CancellationToken cancellationToken = default)
    {
        var delay = sequencer.GetDelayForSendMessage(text);
        await Task.Delay(delay, cancellationToken).FastAwait();

        try
        {
            return await _botClient.SendMessageAsync(id, text,
                parseMode: ParseMode.HTML,
                keyboardMarkup: inlineKeyboard,
                disableNotification: silent,
                disableWebPagePreview: disableWebPagePreview,
                cancellationToken: cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: SendMessage" +
                    $"\n Used delay: {delay} sec" +
                    $"\n Text: {text}");
                throw telegramBotException;
            }
        }
        return new MessageId();
    }

    public async Task EditTextMessage(ChatId id, MessageId messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool disableWebPagePreview = false, CancellationToken cancellationToken = default)
    {
        var delay = sequencer.GetDelayForEditMessage(text);
        await Task.Delay(delay, cancellationToken).FastAwait();

        try
        {
            await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.HTML,
                inlineKeyboardMarkup: inlineKeyboard,
                disableWebPagePreview: disableWebPagePreview,
                cancellationToken: cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: EditMessageText" +
                    $"\n Used delay: {delay} sec" +
                    $"\n Text: {text}");
                throw telegramBotException;
            }
        }
    }

    public async Task DeleteMessage(ChatId id, MessageId messageId)
    {
        try
        {
            await _botClient.DeleteMesageAsync(id, messageId).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: DeleteMesage" +
                    $"\n Used delay: -");
            }
        }
    }

    public async Task EditInlineKeyboardAsync(ChatId id, MessageId messageId, InlineKeyboardMarkup? inlineKeyboard, CancellationToken cancellationToken = default)
    {
        try
        {
            await _botClient.EditInlineKeyboardAsync(id, messageId, inlineKeyboard, cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: EditInlineKeyboard" +
                    $"\n Used delay: -");
            }
        }
    }

    public async Task RemoveInlineKeyboardAsync(ChatId id, MessageId messageId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _botClient.RemoveInlineKeyboardAsync(id, messageId, cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: EditInlineKeyboard (Remove!)" +
                    $"\n Used delay: -");
            }
        }
    }

    public async Task<MessageId> SendTextDialog(ChatId id, string text, ReplyKeyboardMarkup? replyKeyboard = null,
        bool silent = false, bool disableWebPagePreview = false, CancellationToken cancellationToken = default)
    {
        if (replyKeyboard != null)
        {
            replyKeyboard.ResizeKeyboard = true;
        }

        var delay = sequencer.GetDelayForSendMessage(text);
        await Task.Delay(delay, cancellationToken).FastAwait();

        try
        {
            return await _botClient.SendMessageAsync(id, text,
                parseMode: ParseMode.HTML,
                keyboardMarkup: replyKeyboard,
                disableNotification: silent,
                disableWebPagePreview: disableWebPagePreview,
                cancellationToken: cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: SendMessage" +
                    $"\n Used delay: {delay} sec" +
                    $"\n Text: {text}");
                throw telegramBotException;
            }
        }
        return new MessageId();
    }

    public async Task AnswerQuery(string queryId, string? text = null, CancellationToken cancellationToken = default)
    {
        await _botClient.AnswerCallbackQueryAsync(queryId, text, cancellationToken: cancellationToken).FastAwait();
    }

    public async Task SendErrorMessage(ChatId id, string text)
    {
        try
        {
            var delay = sequencer.GetDelayForSendMessage(text);
            await Task.Delay(delay).FastAwait();
            await _botClient.SendMessageAsync(id, Emojis.ElementWarning + "<b>Program Error</b>\n\n" + text, parseMode: ParseMode.HTML).FastAwait();
        }
        catch (Exception)
        {
            // ignored
        }        
    }

    public async Task SendSticker(ChatId id, FileId stickerFileId, CancellationToken cancellationToken = default)
    {
        var delay = sequencer.GetDelayForSendSticker(stickerFileId);
        if (delay >= _maxDelayForSendStickers)
        {
            return;
        }
        await Task.Delay(delay, cancellationToken).FastAwait();

        try
        {
            await _botClient.SendStickerAsync(id, stickerFileId, cancellationToken: cancellationToken).FastAwait();
        }
        catch (TelegramBotException telegramBotException)
        {
            if (telegramBotException.ErrorCode == 429)
            {
                Program.logger.Error("Catched 'Too many requests':" +
                    "\n Method: SendSticker" +
                    $"\n Used delay: {delay} sec" +
                    $"\n FileId: {stickerFileId}");
            }
        }
    }

    public async Task<MessageId> SendDocument(ChatId id, InputFile document, string? caption = null, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendDocumentAsync(id, document, caption: caption, parseMode: ParseMode.HTML, cancellationToken: cancellationToken).FastAwait();
    }

}
