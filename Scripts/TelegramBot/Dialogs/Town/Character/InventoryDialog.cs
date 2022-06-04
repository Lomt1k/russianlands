﻿using TextGameRPG.Scripts.GameCore.Localization;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal class InventoryDialog : DialogBase
    {
        private InventoryInspectorDialogPanel _inspectorPanel;

        public InventoryDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new InventoryInspectorDialogPanel(this, 0);
            RegisterPanel(_inspectorPanel);

            RegisterButton($"{Emojis.items[ItemType.Sword]} " + Localization.Get(session, "menu_item_swords"),
                () => _inspectorPanel.ShowCategory(ItemType.Sword));
            RegisterButton($"{Emojis.items[ItemType.Bow]} " + Localization.Get(session, "menu_item_bows"),
                () => _inspectorPanel.ShowCategory(ItemType.Bow));
            RegisterButton($"{Emojis.items[ItemType.Stick]} " + Localization.Get(session, "menu_item_sticks"),
                () => _inspectorPanel.ShowCategory(ItemType.Stick));
            RegisterButton($"{Emojis.items[ItemType.Helmet]} " + Localization.Get(session, "menu_item_helmets"),
                () => _inspectorPanel.ShowCategory(ItemType.Helmet));
            RegisterButton($"{Emojis.items[ItemType.Armor]} " + Localization.Get(session, "menu_item_armors"),
                () => _inspectorPanel.ShowCategory(ItemType.Armor));
            RegisterButton($"{Emojis.items[ItemType.Boots]} " + Localization.Get(session, "menu_item_boots"),
                () => _inspectorPanel.ShowCategory(ItemType.Boots));
            RegisterButton($"{Emojis.items[ItemType.Shield]} " + Localization.Get(session, "menu_item_shields"),
                () => _inspectorPanel.ShowCategory(ItemType.Shield));
            RegisterButton($"{Emojis.items[ItemType.Amulet]} " + Localization.Get(session, "menu_item_amulets"),
                () => _inspectorPanel.ShowCategory(ItemType.Amulet));
            RegisterButton($"{Emojis.items[ItemType.Ring]} " + Localization.Get(session, "menu_item_rings"),
                () => _inspectorPanel.ShowCategory(ItemType.Ring));
            RegisterButton($"{Emojis.items[ItemType.Poison]} " + Localization.Get(session, "menu_item_poisons"),
                () => _inspectorPanel.ShowCategory(ItemType.Poison));
            RegisterButton($"{Emojis.menuItems[MenuItem.Spells]} " + Localization.Get(session, "menu_item_spells"),
                () => _inspectorPanel.ShowCategory(ItemType.Tome));
            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownCharacterDialog(session).Start());
        }

        public override async Task Start()
        {
            string header = $"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(3, 3, 3, 3));
            await SendPanelsAsync();
        }
    }
}
