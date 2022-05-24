using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class InventoryDialog : DialogBase
    {
        public InventoryDialog(GameSession _session) : base(_session)
        {
            RegisterButton($"{Emojis.inventory[Inventory.Sword]} " + Localization.Get(session, "menu_item_swords"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Bow]} " + Localization.Get(session, "menu_item_bows"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Stick]} " + Localization.Get(session, "menu_item_sticks"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Helmet]} " + Localization.Get(session, "menu_item_helmets"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Armor]} " + Localization.Get(session, "menu_item_armors"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Boots]} " + Localization.Get(session, "menu_item_boots"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Shield]} " + Localization.Get(session, "menu_item_shields"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Amulet]} " + Localization.Get(session, "menu_item_amulets"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Ring]} " + Localization.Get(session, "menu_item_rings"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Poison]} " + Localization.Get(session, "menu_item_poisons"),
                null);
            RegisterButton($"{Emojis.inventory[Inventory.Spells]} " + Localization.Get(session, "menu_item_spells"),
                null);
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_backButton"),
                () => new TownCharacterDialog(session).Start());
        }

        public override async void Start()
        {
            string header = $"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(3, 3, 3, 3));
            await SendPanelsAsync();
        }
    }
}
