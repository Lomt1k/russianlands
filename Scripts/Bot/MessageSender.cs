using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TextGameRPG.Scripts.Bot
{
    public class MessageSender
    {
        private TelegramBotClient _botClient;
        private RequestExceptionHandler _requestExceptionHandler;

        public MessageSender(TelegramBotClient botClient)
        {
            _botClient = botClient;
            _requestExceptionHandler = new RequestExceptionHandler();
        }

        public async Task<Message?> SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null,
            bool silent = false, bool disableWebPagePreview = false)
        {
            try
            {
                return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: inlineKeyboard,
                    disableNotification: silent, disableWebPagePreview: disableWebPagePreview).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

        public async Task<Message?> EditTextMessage(ChatId id, int messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null,
            bool disableWebPagePreview = false)
        {
            try
            {
                return await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.Html, replyMarkup: inlineKeyboard,
                    disableWebPagePreview: disableWebPagePreview).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

        public async Task DeleteMessage(ChatId id, int messageId)
        {
            try
            {
                await _botClient.DeleteMessageAsync(id, messageId).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
            }
        }

        public async Task<Message?> EditMessageKeyboard(ChatId id, int messageId, InlineKeyboardMarkup? inlineKeyboard)
        {
            try
            {
                return await _botClient.EditMessageReplyMarkupAsync(id, messageId, inlineKeyboard).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
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
                return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: replyKeyboard,
                disableNotification: silent, disableWebPagePreview: disableWebPagePreview).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

        public async Task AnswerQuery(ChatId id, string queryId, string? text = null)
        {
            try
            {
                await _botClient.AnswerCallbackQueryAsync(queryId, text).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
            }
        }

        public async Task<Message?> SendErrorMessage(ChatId id, string text)
        {
            try
            {
                return await _botClient.SendTextMessageAsync(id, Emojis.ElementWarning + "<b>Program Error</b>\n\n" + text, ParseMode.Html)
                    .ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

        public async Task<Message?> SendSticker(ChatId id, string stickerFileId)
        {
            try
            {
                return await _botClient.SendStickerAsync(id, stickerFileId).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

        public async Task<Message?> SendDocument(ChatId id, InputOnlineFile document, string? caption = null)
        {
            try
            {
                return await _botClient.SendDocumentAsync(id, document, caption, parseMode: ParseMode.Html).ConfigureAwait(false);
            }
            catch (RequestException ex)
            {
                await _requestExceptionHandler.HandleException(id, ex).ConfigureAwait(false);
                return null;
            }
        }

    }
}
