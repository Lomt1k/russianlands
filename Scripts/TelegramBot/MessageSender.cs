using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task SendTextMessage(ChatId id, string text, bool silent = false, InlineKeyboardMarkup? inlineKeyboard = null)
        {
            await _botClient.SendTextMessageAsync(id, text, disableNotification: silent, replyMarkup: inlineKeyboard);
        }

        public async Task SendEditedMessage(ChatId id, int messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null)
        {
            await _botClient.EditMessageTextAsync(id, messageId, text, replyMarkup: inlineKeyboard);
        }

        public async Task SendTextDialog(ChatId id, string text, bool silent = false, ReplyKeyboardMarkup? replyKeyboard = null)
        {
            await _botClient.SendTextMessageAsync(id, text, disableNotification: silent, replyMarkup: replyKeyboard);
        }

    }
}
