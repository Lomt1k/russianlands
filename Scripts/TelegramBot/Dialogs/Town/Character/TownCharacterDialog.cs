using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class TownCharacterDialog : DialogBase
    {
        public TownCharacterDialog(GameSession _session) : base(_session)
        {
            RegisterButton($"{Emojis.menuItems[MenuItem.Attributes]} " + Localization.Get(session, "menu_item_attributes"),
                () => new AttributesDialog(session).Start());
            RegisterButton($"{Emojis.menuItems[MenuItem.Inventory]} " + Localization.Get(session, "menu_item_inventory"),
                () => new InventoryDialog(session).Start());
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"), 
                () => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        public async override void Start()
        {
            var text = session.player.GetUnitView();
            await messageSender.SendTextDialog(session.chatId, text, GetKeyboardWithRowSizes(2, 1));
        }
    }
}
