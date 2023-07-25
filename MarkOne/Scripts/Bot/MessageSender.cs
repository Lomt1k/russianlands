using System;
using System.Net.Sockets;
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
    private const int attemptsCount = 10;
    private const int attemptDelay = 300;

    private static readonly MessageSequencer sequencer = ServiceLocator.Get<MessageSequencer>();

    private TelegramBotClient _botClient => BotController.botClient;
    private int _maxDelayForSendStickers;

    public override Task OnBotStarted()
    {
        _maxDelayForSendStickers = BotController.config.sendingLimits.dontSendStickerIfDelayInSeconds * 1_000;
        return Task.CompletedTask;
    }

    public async Task<MessageId> SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool silent = false, bool disableWebPagePreview = false, CancellationToken cancellationToken = default)
    {
        var delay = sequencer.GetDelayForSendMessage(text);
        await Task.Delay(delay, cancellationToken).FastAwait();

        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                return await _botClient.SendMessageAsync(id, text,
                    parseMode: ParseMode.HTML,
                    keyboardMarkup: inlineKeyboard,
                    disableNotification: silent,
                    disableWebPagePreview: disableWebPagePreview,
                    cancellationToken: resultCTS.Token).FastAwait();
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx 
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }
        
        return default;
    }

    public async Task EditTextMessage(ChatId id, MessageId messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool disableWebPagePreview = false, CancellationToken cancellationToken = default)
    {
        var delay = sequencer.GetDelayForEditMessage(text);
        await Task.Delay(delay, cancellationToken).FastAwait();

        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.HTML,
                    inlineKeyboardMarkup: inlineKeyboard,
                    disableWebPagePreview: disableWebPagePreview,
                    cancellationToken: resultCTS.Token).FastAwait();
                return;
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }            
    }

    public async Task DeleteMessage(ChatId id, MessageId messageId)
    {
        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            try
            {
                await _botClient.DeleteMesageAsync(id, messageId, currentRequestCTS.Token).FastAwait();
                return;
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }            
    }

    public async Task EditInlineKeyboardAsync(ChatId id, MessageId messageId, InlineKeyboardMarkup? inlineKeyboard, CancellationToken cancellationToken = default)
    {
        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                await _botClient.EditInlineKeyboardAsync(id, messageId, inlineKeyboard, resultCTS.Token).FastAwait();
                return;
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }
    }

    public async Task RemoveInlineKeyboardAsync(ChatId id, MessageId messageId, CancellationToken cancellationToken = default)
    {
        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                await _botClient.RemoveInlineKeyboardAsync(id, messageId, resultCTS.Token).FastAwait();
                return;
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
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

        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                return await _botClient.SendMessageAsync(id, text,
                    parseMode: ParseMode.HTML,
                    keyboardMarkup: replyKeyboard,
                    disableNotification: silent,
                    disableWebPagePreview: disableWebPagePreview,
                    cancellationToken: resultCTS.Token).FastAwait();
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }
            
        return default;
    }

    public async Task AnswerQuery(string queryId, string? text = null, CancellationToken cancellationToken = default)
    {
        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                await _botClient.AnswerCallbackQueryAsync(queryId, text, cancellationToken: resultCTS.Token).FastAwait();
                return;
            }
            catch (TelegramBotException telegramBotException)
            {
                if (telegramBotException.ErrorCode == 400)
                {
                    return;
                }
                throw telegramBotException;
            }
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }            
    }

    public async Task SendErrorMessage(ChatId id, string text)
    {
        var currentRequestCTS = new CancellationTokenSource();
        try
        {
            var delay = sequencer.GetDelayForSendMessage(text);
            await Task.Delay(delay).FastAwait();
            await _botClient.SendMessageAsync(id, Emojis.ElementWarning + "<b>Program Error</b>\n\n" + text,
                parseMode: ParseMode.HTML, cancellationToken: currentRequestCTS.Token).FastAwait();
        }
        catch (Exception)
        {
            currentRequestCTS.Cancel();
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

        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                await _botClient.SendStickerAsync(id, stickerFileId, cancellationToken: resultCTS.Token).FastAwait();
                return;
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
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }            
    }

    public async Task<MessageId> SendDocument(ChatId id, InputFile document, string? caption = null, CancellationToken cancellationToken = default)
    {
        SocketException? cachedSocketException = null;
        for (int i = 1; i <= attemptsCount; i++)
        {
            var currentRequestCTS = new CancellationTokenSource();
            var resultCTS = CancellationTokenSource.CreateLinkedTokenSource(currentRequestCTS.Token, cancellationToken);
            try
            {
                return await _botClient.SendDocumentAsync(id, document, caption: caption, parseMode: ParseMode.HTML, cancellationToken: resultCTS.Token).FastAwait();
            }
            catch (Exception ex)
            {
                currentRequestCTS.Cancel();
                cachedSocketException ??= ex is SocketException socketEx ? socketEx
                        : ex.InnerException is SocketException innerSocketEx ? innerSocketEx
                        : null;
                if (i < attemptsCount)
                {
                    await Task.Delay(attemptDelay).FastAwait();
                    continue;
                }
                throw cachedSocketException ?? ex;
            }
        }

        return default;
    }

}
