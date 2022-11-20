using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialog : DialogBase
    {
        private int? _regenHealthMessageId;

        public TownCharacterDialog(GameSession _session) : base(_session)
        {
            RegisterButton($"{Emojis.menuItems[MenuItem.Avatar]} " + Localization.Get(session, "menu_item_avatar"),
                   () => messageSender.SendTextMessage(session.chatId, "Смена аватара недоступна в текущей версии игры")); //заглушка
            RegisterButton($"{Emojis.menuItems[MenuItem.Inventory]} " + Localization.Get(session, "menu_item_inventory"),
                () => new InventoryDialog(session).Start());
            RegisterTownButton(isFullBack: false);
        }

        public override async Task Start()
        {
            session.player.healhRegenerationController.InvokeRegen();

            var sb = new StringBuilder();
            sb.AppendLine(session.player.GetGeneralUnitInfoView(session));
            bool isFullHealth = session.player.unitStats.isFullHealth;
            sb.AppendLine(session.player.unitStats.GetView(session, isFullHealth));
            TryAppendTooltip(sb);

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(2, 1))
                .ConfigureAwait(false);

            if (!isFullHealth)
            {
                await SendHealthRegenMessage()
                    .ConfigureAwait(false);
            }
        }

        private async Task SendHealthRegenMessage()
        {
            try
            {
                var sb = new StringBuilder();
                var stats = session.player.unitStats;
                if (_regenHealthMessageId.HasValue)
                {
                    if (session.currentDialog != this)
                    {
                        await messageSender.DeleteMessage(session.chatId, _regenHealthMessageId.Value)
                            .ConfigureAwait(false);
                        return;
                    }
                    else if (stats.currentHP >= stats.maxHP)
                    {
                        sb.AppendLine(Localization.Get(session, "unit_view_health"));
                        sb.AppendLine($"{Emojis.stats[Stat.Health]} {stats.currentHP} / {stats.maxHP}");
                        await messageSender.EditTextMessage(session.chatId, _regenHealthMessageId.Value, sb.ToString())
                            .ConfigureAwait(false);
                        return;
                    }
                }

                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "unit_view_health_regen"));
                sb.AppendLine($"{Emojis.stats[Stat.Health]} {stats.currentHP} / {stats.maxHP}");

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
