using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    public struct CompareData
    {
        public InventoryItem comparedItem;
        public ItemType categoryOnStartCompare;
        public int currentPageOnStartCompare;
    }

    public class InventoryInspectorDialogPanel : DialogPanelBase
    {
        private const int browsedItemsOnPage = 8;

        public CompareData? compareData;

        private PlayerInventory _inventory;
        private ItemType _browsedCategory;
        private InventoryItem[] _browsedItems;
        private int _currentPage;
        private int _pagesCount;

        public InventoryInspectorDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            _inventory = session.player.inventory;
        }

        private string BuildMainItemsInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format(Localization.Get(session, "dialog_inventory_header_total_items"), _inventory.itemsCount, _inventory.inventorySize));

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Sword]} {_inventory.GetItemsCountByType(ItemType.Sword)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Bow]} {_inventory.GetItemsCountByType(ItemType.Bow)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Stick]} {_inventory.GetItemsCountByType(ItemType.Stick)}");


            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Helmet]} {_inventory.GetItemsCountByType(ItemType.Helmet)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Armor]} {_inventory.GetItemsCountByType(ItemType.Armor)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Boots]} {_inventory.GetItemsCountByType(ItemType.Boots)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Shield]} {_inventory.GetItemsCountByType(ItemType.Shield)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Amulet]} {_inventory.GetItemsCountByType(ItemType.Amulet)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Ring]} {_inventory.GetItemsCountByType(ItemType.Ring)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Scroll]} {_inventory.GetItemsCountByType(ItemType.Scroll)}");
            //sb.Append(Emojis.bigSpace);
            //sb.AppendLine($"{Emojis.items[ItemType.Poison]} {_inventory.GetItemsCountByType(ItemType.Poison)}");

            return sb.ToString();
        }

        public override async Task SendAsync()
        {
            await ShowMainInfo()
                .ConfigureAwait(false);
        }

        public async Task ShowMainInfo()
        {
            await RemoveKeyboardFromLastMessage()
                .ConfigureAwait(false);

            RegisterButton($"{Emojis.items[ItemType.Equipped]} {Localization.Get(session, "menu_item_equipped")}",
                () => ((InventoryDialog)dialog).ShowCategory(ItemType.Equipped));

            var sb = new StringBuilder();
            sb.Append(BuildMainItemsInfo());

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetMultilineKeyboard(), asNewMessage: true)
                .ConfigureAwait(false);
        }

        public async Task ShowCategory(ItemType category, int itemsPage = 0)
        {
            _browsedCategory = category;
            RefreshBrowsedItems();
            _currentPage = itemsPage;
            await ShowItemsPage(asNewMessage: true)
                .ConfigureAwait(false);
        }

        private void RefreshBrowsedItems()
        {
            switch (_browsedCategory)
            {
                case ItemType.Equipped:
                    _browsedItems = _inventory.equipped.allEquipped;
                    break;
                default:
                    _browsedItems = _inventory.GetItemsByType(_browsedCategory).ToArray();
                    break;
            }

            _currentPage = 0;
            _pagesCount = 0;

            if (_browsedItems.Length < 1)
                return;

            _pagesCount = _browsedItems.Length % browsedItemsOnPage > 0
                ? _browsedItems.Length / browsedItemsOnPage + 1
                : _browsedItems.Length / browsedItemsOnPage;
        }

        private async Task ShowItemsPage(bool asNewMessage)
        {
            ClearButtons();
            var categoryLocalization = GetCategoryLocalization(_browsedCategory);
            var text = new StringBuilder();
            text.Append($"<b>{categoryLocalization}:</b> ");

            if (_pagesCount == 0)
            {
                text.Append(Localization.Get(session, "dialog_inventory_has_not_items"));
            }
            else
            {
                text.AppendLine(_browsedItems.Length.ToString());
                int startIndex = _currentPage * browsedItemsOnPage;
                for (int i = startIndex; i < startIndex + browsedItemsOnPage && i < _browsedItems.Length; i++)
                {
                    var item = _browsedItems[i];
                    var prefix = item.isEquipped ? Emojis.items[ItemType.Equipped] : string.Empty;
                    RegisterButton(prefix + item.GetFullName(session), () => OnItemClick(item));
                }
            }

            if (_pagesCount > 1)
            {
                text.AppendLine(string.Format(Localization.Get(session, "dialog_inventory_current_page"), _currentPage + 1, _pagesCount));
                if (_currentPage > 0)
                    RegisterButton("<<", () => OnClickPreviousPage());
                if (_currentPage < _pagesCount - 1)
                    RegisterButton(">>", () => OnClickNextPage());
            }

            TryAppendTooltip(text);
            await SendPanelMessage(text, GetItemsPageKeyboard(), asNewMessage)
                .ConfigureAwait(false);
        }

        private string GetCategoryLocalization(ItemType category)
        {
            switch (category)
            {
                case ItemType.Equipped:
                    return Localization.Get(session, $"menu_item_equipped");
                default:
                    string stringCategory = category.ToString().ToLower();
                    if (!stringCategory.EndsWith('s'))
                    {
                        stringCategory = stringCategory + 's';
                    }
                    return Localization.Get(session, $"menu_item_{stringCategory}");
            }            
        }

        private async Task OnItemClick(InventoryItem item)
        {
            if (compareData != null)
            {
                await new InventoryItemComparisonDialog(session, item, compareData.Value).Start()
                    .ConfigureAwait(false);
                return;
            }
            await new InventoryItemDialog(session, item, _browsedCategory, _currentPage).Start()
                .ConfigureAwait(false);
        }

        private async Task OnClickPreviousPage()
        {
            _currentPage--;
            await ShowItemsPage(asNewMessage: false)
                .ConfigureAwait(false);
        }

        private async Task OnClickNextPage()
        {
            _currentPage++;
            await ShowItemsPage(asNewMessage: false)
                .ConfigureAwait(false);
        }

        private InlineKeyboardMarkup GetItemsPageKeyboard()
        {
            if (_pagesCount < 2)
                return GetMultilineKeyboard();

            int lastRowButtons = _currentPage == _pagesCount - 1 || _currentPage == 0 ? 1 : 2;
            var parameters = new int[buttonsCount - lastRowButtons + 1];
            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = i < parameters.Length - 1 ? 1 : lastRowButtons;
            }
            return GetKeyboardWithRowSizes(parameters);
        }

    }
}
