using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialog : DialogBase
    {
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

            bool withTooltip = session.tooltipController.HasTooltipToAppend(this);
            if (withTooltip)
            {
                sb.AppendLine(session.player.unitStats.GetView(session));
                TryAppendTooltip(sb);
            }
            else
            {
                RegisterPanel(new TownCharacterDialogPanel(this, 0));
            }

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(2, 1))
                .ConfigureAwait(false);
            await SendPanelsAsync()
                .ConfigureAwait(false);
        }

    }
}
