using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;

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
    private readonly TownEntryReason _reason;
    private readonly ReplyKeyboardMarkup _keyboard;
    private int? _regenHealthMessageId;

    public TownDialog(GameSession _session, TownEntryReason reason) : base(_session)
    {
        _reason = reason;
        var player = session.player;
        var hasTooltip = session.tooltipController.HasTooltipToAppend(this);

        RegisterButton(Emojis.ButtonMap + Localization.Get(session, "menu_item_map"),
            () => new Map.MapDialog(session).Start());

        RegisterButton(Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings"),
            () => notificationsManager.GetNotificationsAndOpenBuildingsDialog(session));

        var characterButton = Emojis.AvatarMale + Localization.Get(session, "menu_item_character")
            + (player.inventory.hasAnyNewItem && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
        RegisterButton(characterButton, () => new Character.CharacterDialog(session).Start());

        RegisterButton(Emojis.ButtonQuests + Localization.Get(session, "menu_item_quests"),
            () => messageSender.SendTextMessage(session.chatId, "Задания недоступны в текущей версии игры")); //заглушка
        RegisterButton(Emojis.ButtonMail + Localization.Get(session, "menu_item_mail"),
            () => messageSender.SendTextMessage(session.chatId, "Почта недоступна в текущей версии игры")); //заглушка
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
            () => new Shop.ShopDialog(session).Start());

        _keyboard = GetKeyboardWithRowSizes(1, 2, 3);
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
        if (!withTooltip)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_town_text_backFromInnerDialog"));
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

            var message = _regenHealthMessageId == null
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), silent: true, cancellationToken: session.cancellationToken).FastAwait()
                : await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString(), cancellationToken: session.cancellationToken).FastAwait();
            _regenHealthMessageId = message?.MessageId;

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
