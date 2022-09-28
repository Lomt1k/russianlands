using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class TownCharacterDialog : DialogBase
    {
        public TownCharacterDialog(GameSession _session) : base(_session)
        {
            RegisterPanel(new TownCharacterDialogPanel(this, 0));

            RegisterButton($"{Emojis.menuItems[MenuItem.Avatar]} " + Localization.Get(session, "menu_item_avatar"),
                null);
            RegisterButton($"{Emojis.menuItems[MenuItem.Inventory]} " + Localization.Get(session, "menu_item_inventory"),
                () => new InventoryDialog(session).Start());
            RegisterBackButton(() => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public override async Task Start()
        {
            string header = session.player.GetGeneralUnitInfoView(session);
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(2, 1));
            await SendPanelsAsync();
        }
    }
}
