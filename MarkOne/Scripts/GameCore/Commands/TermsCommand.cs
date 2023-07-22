using FastTelegramBot.DataTypes.Keyboards;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Commands;
public class TermsCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.ForAll;

    public override async Task Execute(GameSession session, string[] args)
    {
        var text = Localization.Get(session, "terms_notification_header");
        var buttonText = Localization.Get(session, "terms_link_button");
        var url = BotController.config.termsUrl;

        var button = InlineKeyboardButton.WithUrl(buttonText, url);
        var markup = new InlineKeyboardMarkup(new[] { button });
        await messageSender.SendTextMessage(session.chatId, text, markup).FastAwait();
    }
}
