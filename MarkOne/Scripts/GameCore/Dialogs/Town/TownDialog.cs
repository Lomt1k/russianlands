using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Town.Map;
using MarkOne.Scripts.GameCore.Services.News;
using MarkOne.Scripts.GameCore.Services;
using FastTelegramBot.DataTypes;
using FastTelegramBot.DataTypes.Keyboards;
using System.Linq;
using System;
using MarkOne.Scripts.GameCore.Shop.Offers;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Dialogs.Events;

namespace MarkOne.Scripts.GameCore.Dialogs.Town;

public enum TownEntryReason
{
    StartNewSession,
    BackFromInnerDialog,
    FromQuestAction,
    BattleEnd
}

public class TownDialog : DialogBase
{
    private static readonly string linksText = string.Empty;

    private readonly NewsService newsService = ServiceLocator.Get<NewsService>();

    private readonly TownEntryReason _reason;
    private readonly ReplyKeyboardMarkup _keyboard;
    private MessageId? _regenHealthMessageId;

    static TownDialog()
    {
        var sb = new StringBuilder();
        foreach (var link in BotController.config.socialLinks)
        {
            sb.AppendLine($@"<a href=""{link.url}"">{link.description}</a>".Bold());
        }
        linksText = sb.ToString();
    }

    public TownDialog(GameSession _session, TownEntryReason reason) : base(_session)
    {
        _reason = reason;
        var player = session.player;
        var hasTooltip = session.tooltipController.HasTooltipToAppend(this);

        var mapButton = Emojis.ButtonMap + Localization.Get(session, "menu_item_map")
            + (MapDialog.HasNewActivities(session) ? Emojis.ElementWarningRed.ToString() : string.Empty);
        RegisterButton(mapButton, () => new MapDialog(session).Start());

        RegisterButton(Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings"),
            () => notificationsManager.GetNotificationsAndOpenBuildingsDialog(session));

        var characterButton = Emojis.AvatarMale + Localization.Get(session, "menu_item_character")
            + (player.inventory.hasAnyNewItem && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
        RegisterButton(characterButton, () => new Character.CharacterDialog(session).Start());

        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
            () => new Shop.ShopDialog(session).Start());

        var newsButton = (newsService.HasNew(session) && !hasTooltip ? Emojis.ElementWarningRed.ToString() : Emojis.ButtonNews) 
            + Localization.Get(session, "menu_item_news");
        RegisterButton(newsButton, () => new News.NewsDialog(session).Start());

        var eventsButton = (EventsDialog.HasNew(session) && !hasTooltip ? Emojis.ElementWarningRed.ToString() : Emojis.ButtonEvents)
            + Localization.Get(session, "menu_item_events");
        RegisterButton(eventsButton, () => new EventsDialog(session).Start());

        var hasOfferButton = TryRegisterRandomOfferButton();
        _keyboard = hasOfferButton ? GetKeyboardWithRowSizes(1, 2, 3, 1) : GetKeyboardWithRowSizes(1, 2, 3);
    }

    private bool TryRegisterRandomOfferButton()
    {
        var activeOffers = session.profile.dynamicData.offers.Where(x => x.IsActive()).ToArray();
        if (activeOffers.Length < 1)
        {
            return false;
        }

        var randIndex = new Random().Next(activeOffers.Length);
        var offer = activeOffers[randIndex];
        var buttonText = offer.GetTitleForList(session);
        RegisterButton(buttonText, () => ShowOffer(offer));
        return true;
    }

    private async Task ShowOffer(OfferItem offerItem)
    {
        var offerData = offerItem.GetData();
        await offerData.StartOfferDialog(session, offerItem, () => new ShopDialog(session).StartWithCategory(ShopCategory.Offers));
    }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Emojis.ButtonTown + Localization.Get(session, "menu_item_town").Bold());
        sb.AppendLine();

        if (_reason == TownEntryReason.StartNewSession)
        {
            sb.AppendLine(Localization.Get(session, "dialog_town_text_newSession"));
            sb.AppendLine();
        }
        var resources = session.player.resources.GetGeneralResourcesView();
        sb.AppendLine(resources);

        var withTooltip = TryAppendTooltip(sb);
        if (!withTooltip && linksText.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_town_helphul_links_header"));
            sb.Append(linksText);
        }

        await SendDialogMessage(sb, _keyboard).FastAwait();

        session.player.healhRegenerationController.InvokeRegen();
        if (!session.player.unitStats.isFullHealth)
        {
            await SendHealthRegenMessage().FastAwait();
        }
    }

    private async Task SendHealthRegenMessage()
    {
        try
        {
            var stats = session.player.unitStats;
            if (stats.currentHP >= stats.maxHP || session.currentDialog != this)
            {
                if (_regenHealthMessageId.HasValue)
                {
                    await messageSender.DeleteMessage(session.chatId, _regenHealthMessageId.Value).FastAwait();
                }
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "unit_view_health_regen"));
            sb.AppendLine(Emojis.StatHealth + $"{stats.currentHP} / {stats.maxHP}");

            if (_regenHealthMessageId is null)
            {
                _regenHealthMessageId = await messageSender.SendTextMessage(session.chatId, sb.ToString(), silent: true, cancellationToken: session.cancellationToken).FastAwait();
            }
            else
            {
                await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString(), cancellationToken: session.cancellationToken).FastAwait();
            }

            WaitOneSecondAndInvokeHealthRegen();
        }
        catch (System.Exception) { } //ignored
    }

    private async void WaitOneSecondAndInvokeHealthRegen()
    {
        try
        {
            await Task.Delay(1_000).FastAwait();
            if (session.cancellationToken.IsCancellationRequested)
                return;

            session.player.healhRegenerationController.InvokeRegen();
            await SendHealthRegenMessage().FastAwait();
        }
        catch (System.Exception) { } //ignored
    }

}
