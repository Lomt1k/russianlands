using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town
{
    public enum TownEntryReason
    {
        StartNewSession,
        BackFromInnerDialog,
        FromQuestAction
    }

    public class TownDialog : DialogBase
    {
        private TownEntryReason _reason;
        private ReplyKeyboardMarkup _keyboard;
        private int? _regenHealthMessageId;

        public TownDialog(GameSession _session, TownEntryReason reason) : base(_session)
        {
            _reason = reason;
            var player = session.player;
            var hasTooltip = session.tooltipController.HasTooltipToAppend(this);

            RegisterButton(Emojis.ButtonMap + Localization.Get(session, "menu_item_map"),
                () => new GlobalMap.GlobalMapDialog(session).Start());

            var buildingsLocalization = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings")
                + (player.buildings.HasImportantUpdates() && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
            RegisterButton(buildingsLocalization, () => new Buildings.BuildingsDialog(session).Start());

            var characterButton = Emojis.AvatarMale + Localization.Get(session, "menu_item_character")
                + (player.inventory.hasAnyNewItem && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
            RegisterButton(characterButton, () => new Character.TownCharacterDialog(session).Start());

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
            string resources = session.player.resources.GetGeneralResourcesView();
            sb.AppendLine(resources);

            bool withTooltip = TryAppendTooltip(sb);
            if (!withTooltip)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "dialog_town_text_backFromInnerDialog"));
            }

            await SendDialogMessage(sb, _keyboard)
                    .ConfigureAwait(false);

            session.player.healhRegenerationController.InvokeRegen();
            if (!session.player.unitStats.isFullHealth)
            {
                await SendHealthRegenMessage()
                    .ConfigureAwait(false);
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
                        await messageSender.DeleteMessage(session.chatId, _regenHealthMessageId.Value)
                            .ConfigureAwait(false);
                    }
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "unit_view_health_regen"));
                sb.AppendLine(Emojis.StatHealth + $"{stats.currentHP} / {stats.maxHP}");

                var message = _regenHealthMessageId == null
                    ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), silent: true).ConfigureAwait(false)
                    : await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString()).ConfigureAwait(false);
                _regenHealthMessageId = message?.MessageId;

                WaitOneSecondAndInvokeHealthRegen();
            }
            catch (System.Exception ex) { } //ignored
        }

        private async void WaitOneSecondAndInvokeHealthRegen()
        {
            try
            {
                await Task.Delay(1_000).ConfigureAwait(false);
                if (session.IsTasksCancelled())
                    return;

                session.player.healhRegenerationController.InvokeRegen();
                await SendHealthRegenMessage().ConfigureAwait(false);
            }
            catch (System.Exception ex) { } //ignored
        }

    }
}
