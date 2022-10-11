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
                   null);
            RegisterButton($"{Emojis.menuItems[MenuItem.Inventory]} " + Localization.Get(session, "menu_item_inventory"),
                () => new InventoryDialog(session).Start());
            RegisterBackButton(() => new TownDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
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

            await messageSender.SendTextDialog(session.chatId, sb.ToString(), GetKeyboardWithRowSizes(2, 1));
            await SendPanelsAsync();
        }

    }
}
