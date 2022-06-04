﻿using System.Threading.Tasks;
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

        public async Task<Message> SendTextMessage(ChatId id, string text, InlineKeyboardMarkup? inlineKeyboard = null, bool silent = false)
        {
            return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: inlineKeyboard, disableNotification: silent);
        }

        public async Task<Message> EditTextMessage(ChatId id, int messageId, string text, InlineKeyboardMarkup? inlineKeyboard = null)
        {
            return await _botClient.EditMessageTextAsync(id, messageId, text, ParseMode.Html, replyMarkup: inlineKeyboard);
        }

        public async Task DeleteMessage(ChatId id, int messageId)
        {
            await _botClient.DeleteMessageAsync(id, messageId);
        }

        public async Task<Message> EditMessageKeyboard(ChatId id, int messageId, InlineKeyboardMarkup? inlineKeyboard)
        {
            return await _botClient.EditMessageReplyMarkupAsync(id, messageId, inlineKeyboard);
        }

        public async Task<Message> SendTextDialog(ChatId id, string text, ReplyKeyboardMarkup? replyKeyboard = null, bool silent = false)
        {
            return await _botClient.SendTextMessageAsync(id, text, ParseMode.Html, replyMarkup: replyKeyboard, disableNotification: silent);
        }

        public async Task AnswerQuery(string queryId, string? text = null)
        {
            await _botClient.AnswerCallbackQueryAsync(queryId, text);
        }

        public async Task<Message> SendErrorMessage(ChatId id, string text)
        {
            return await _botClient.SendTextMessageAsync(id, $"{Emojis.elements[Element.Warning]} Program Error\n\n" + text, ParseMode.Html);
        }

    }
}
