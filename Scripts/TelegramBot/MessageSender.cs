using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TextGameRPG.Scripts.TelegramBot
{
    public class MessageSender
    {
        private TelegramBotClient _botClient;

        public MessageSender(TelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool silent = false)
        {
            await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: inlineKeyboard, disableNotification: silent);
        }

        public async Task SendEditedMessage(ChatId id, int messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null)
        {
            await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.Html, replyMarkup: inlineKeyboard);
        }

        public async Task SendTextDialog(ChatId id, string text, ReplyKeyboardMarkup? replyKeyboard = null, bool silent = false)
        {
            await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: replyKeyboard, disableNotification: silent);
        }

    }
}
