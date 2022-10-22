using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using System.Threading.Tasks;
using System.Text;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public class InventoryDialog : DialogBase
    {
        private InventoryInspectorDialogPanel _inspectorPanel;

        public InventoryDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new InventoryInspectorDialogPanel(this, 0);
            RegisterPanel(_inspectorPanel);
        }

        public override async Task Start()
        {
            ClearButtons();
            RegisterButton($"{Emojis.items[ItemType.Sword]} " + Localization.Get(session, "menu_item_swords"),
                () => ShowCategory(ItemType.Sword));
            RegisterButton($"{Emojis.items[ItemType.Bow]} " + Localization.Get(session, "menu_item_bows"),
                () => ShowCategory(ItemType.Bow));
            RegisterButton($"{Emojis.items[ItemType.Stick]} " + Localization.Get(session, "menu_item_sticks"),
                () => ShowCategory(ItemType.Stick));
            RegisterButton($"{Emojis.items[ItemType.Helmet]} " + Localization.Get(session, "menu_item_helmets"),
                () => ShowCategory(ItemType.Helmet));
            RegisterButton($"{Emojis.items[ItemType.Armor]} " + Localization.Get(session, "menu_item_armors"),
                () => ShowCategory(ItemType.Armor));
            RegisterButton($"{Emojis.items[ItemType.Boots]} " + Localization.Get(session, "menu_item_boots"),
                () => ShowCategory(ItemType.Boots));
            RegisterButton($"{Emojis.items[ItemType.Shield]} " + Localization.Get(session, "menu_item_shields"),
                () => ShowCategory(ItemType.Shield));
            RegisterButton($"{Emojis.items[ItemType.Amulet]} " + Localization.Get(session, "menu_item_amulets"),
                () => ShowCategory(ItemType.Amulet));
            RegisterButton($"{Emojis.items[ItemType.Ring]} " + Localization.Get(session, "menu_item_rings"),
                () => ShowCategory(ItemType.Ring));
            RegisterButton($"{Emojis.items[ItemType.Scroll]} " + Localization.Get(session, "menu_item_scrolls"),
                () => ShowCategory(ItemType.Scroll));
            RegisterButton($"{Emojis.items[ItemType.Poison]} " + Localization.Get(session, "menu_item_poisons"),
                () => ShowCategory(ItemType.Poison));

            RegisterBackButton(() => new TownCharacterDialog(session).Start());

            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>");
            bool hasTooltip = TryAppendTooltip(sb);

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 3, 3, 3));
            if (!hasTooltip)
            {
                await SendPanelsAsync();
            }
        }

        public async Task ShowCategory(ItemType category)
        {
            ClearButtons();
            RegisterBackButton(() => Start());

            var text = $"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>";
            await SendDialogMessage(text, GetOneLineKeyboard());
            await _inspectorPanel.ShowCategory(category);
        }

    }
}
