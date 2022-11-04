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
            await ShowCategories();
        }

        private async Task ShowCategories(CompareData? compareData = null)
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
            //RegisterButton($"{Emojis.items[ItemType.Poison]} " + Localization.Get(session, "menu_item_poisons"),
            //    () => ShowCategory(ItemType.Poison));

            RegisterBackButton(() => new TownCharacterDialog(session).Start());
            RegisterTownButton(isFullBack: true);

            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>");
            bool hasTooltip = TryAppendTooltip(sb);

            await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 3, 3, 3));
            if (!hasTooltip)
            {
                _inspectorPanel.compareData = compareData;
                await _inspectorPanel.ShowMainInfo();
            }
        }

        public async Task ShowCategory(ItemType category, int page = 0, CompareData? newCompareData = null)
        {
            _inspectorPanel.OnDialogClose(); // Чтобы убрать кнопку "Экипированное"
            if (newCompareData.HasValue)
            {
                _inspectorPanel.compareData = newCompareData.Value;
            }

            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_inventory")} {Emojis.menuItems[MenuItem.Inventory]}",
                () => ShowCategories(_inspectorPanel.compareData));
            RegisterTownButton(isFullBack: true);

            var sb = new StringBuilder();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Inventory]} <b>{Localization.Get(session, "menu_item_inventory")}</b>");
            if (_inspectorPanel.compareData.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "menu_item_compare_button_header"));
            }

            await SendDialogMessage(sb, GetOneLineKeyboard());
            await _inspectorPanel.ShowCategory(category, page);
        }

    }
}
