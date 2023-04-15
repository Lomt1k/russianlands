using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.Sending;

namespace TextGameRPG.Scripts.Bot
{
    public class MessageSender : Service
    {
        private static readonly MessageSequencer sequencer = Services.Get<MessageSequencer>();

        private TelegramBotClient _botClient;
        private RequestExceptionHandler _requestExceptionHandler;
        private int _maxDelayForSendStickers;

        public override void OnBotStarted(TelegramBot bot)
        {
            _botClient = bot.botClient;
            _requestExceptionHandler = new RequestExceptionHandler();
            _maxDelayForSendStickers = BotConfig.instance.dontSendStickerIfDelayInSeconds * 1_000;
        }

        public async Task<Message?> SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null,
            bool silent = false, bool disableWebPagePreview = false)
        {
            try
            {
                var delay = sequencer.GetDelayForSendMessage(text);
                await Task.Delay(delay).FastAwait();

                return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: inlineKeyboard,
                    disableNotification: silent, disableWebPagePreview: disableWebPagePreview).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task<Message?> EditTextMessage(ChatId id, int messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null,
            bool disableWebPagePreview = false)
        {
            try
            {
                var delay = sequencer.GetDelayForEditMessage(text);
                await Task.Delay(delay).FastAwait();

                return await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.Html, replyMarkup: inlineKeyboard,
                    disableWebPagePreview: disableWebPagePreview).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task DeleteMessage(ChatId id, int messageId)
        {
            try
            {
                await _botClient.DeleteMessageAsync(id, messageId).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
            }
        }

        public async Task<Message?> EditMessageKeyboard(ChatId id, int messageId, InlineKeyboardMarkup? inlineKeyboard)
        {
            try
            {
                return await _botClient.EditMessageReplyMarkupAsync(id, messageId, inlineKeyboard).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task<Message?> SendTextDialog(ChatId id, string text, ReplyKeyboardMarkup? replyKeyboard = null,
            bool silent = false, bool disableWebPagePreview = false)
        {
            if (replyKeyboard != null)
            {
                replyKeyboard.ResizeKeyboard = true;
            }

            try
            {
                var delay = sequencer.GetDelayForSendMessage(text);
                await Task.Delay(delay).FastAwait();

                return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: replyKeyboard,
                disableNotification: silent, disableWebPagePreview: disableWebPagePreview).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task AnswerQuery(ChatId id, string queryId, string? text = null)
        {
            try
            {
                await _botClient.AnswerCallbackQueryAsync(queryId, text).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
            }
        }

        public async Task<Message?> SendErrorMessage(ChatId id, string text)
        {
            try
            {
                var delay = sequencer.GetDelayForSendMessage(text);
                await Task.Delay(delay).FastAwait();

                return await _botClient.SendTextMessageAsync(id, Emojis.ElementWarning + "<b>Program Error</b>\n\n" + text, ParseMode.Html)
                    .FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task SendSticker(ChatId id, string stickerFileId)
        {
            try
            {
                var delay = sequencer.GetDelayForSendSticker(stickerFileId);
                if (delay >= _maxDelayForSendStickers)
                {
                    return;
                }
                await Task.Delay(delay).FastAwait();
                await _botClient.SendStickerAsync(id, stickerFileId).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
            }
        }

        public async Task<Message?> SendDocument(ChatId id, InputOnlineFile document, string? caption = null)
        {
            try
            {
                return await _botClient.SendDocumentAsync(id, document, caption, parseMode: ParseMode.Html).FastAwait();
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).FastAwait();
                return null;
            }
        }

        public async Task<File?> GetFileAsync(string fileId)
        {
            try
            {
                return await _botClient.GetFileAsync(fileId).FastAwait();
            }
            catch (RequestException ex)
            {
                Program.logger.Error($"ApiRequestException: {ex.Message}");
                return null;
            }
        }

        public async Task DownloadFileAsync(string filePath, System.IO.Stream destination)
        {
            try
            {
                await _botClient.DownloadFileAsync(filePath, destination).FastAwait();
            }
            catch (RequestException ex)
            {
                Program.logger.Error($"ApiRequestException: {ex.Message}");
            }
        }

    }
}
