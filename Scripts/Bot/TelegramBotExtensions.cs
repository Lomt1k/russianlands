using MarkOne.Scripts.Bot;
using Telegram.Bot.Types;

public static class TelegramBotExtensions
{
    public static SimpleUpdate ToSimple(this Update update)
    {
        return new SimpleUpdate()
        {
            id = update.Id,
            updateType = update.Type,
            message = update.Message.ToSimple(),
            callbackQuery = update.CallbackQuery.ToSimple(),
        };
    }

    public static SimpleMessage? ToSimple(this Message? message)
    {
        if (message is null || message.From is null)
        {
            return null;
        }
        return new SimpleMessage
        {
            id = message.MessageId,
            from = message.From.ToSimple(),
            date = message.Date,
            text = message.Text,
            document = message.Document.ToSimple(),
        };
    }

    public static SimpleCallbackQuery? ToSimple(this CallbackQuery? callbackQuery)
    {
        if (callbackQuery is null)
        {
            return null;
        }
        return new SimpleCallbackQuery
        {
            id = callbackQuery.Id,
            from = callbackQuery.From.ToSimple(),
            data = callbackQuery.Data,
        };
    }

    public static SimpleUser ToSimple(this User user)
    {
        return new SimpleUser
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            username = user.Username,
        };
    }

    public static SimpleDocument ToSimple(this Document? document)
    {
        if (document is null)
        {
            return null;
        }
        // TODO
        return null;
    }



}
