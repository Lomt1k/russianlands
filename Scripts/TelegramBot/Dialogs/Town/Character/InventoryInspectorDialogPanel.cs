using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localization;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Character
{
    internal struct CompareData
    {
        public InventoryItem comparedItem;
        public Message comparedItemMessage;
        public ItemType? categoryOnStartCompare;
        public int currentPageOnStartCompare;
        public int pagesCountOnStartCompare;
    }

    internal class InventoryInspectorDialogPanel : DialogPanelBase
    {
        private const int browsedItemsOnPage = 5;

        private readonly string _mainItemsInfo;

        private PlayerInventory _inventory;
        private Message? _lastMessage;

        private ItemType? _browsedCategory;
        private InventoryItem[]? _browsedItems;
        private int _currentPage;
        private int _pagesCount;

        private CompareData? _compareData;

        public InventoryInspectorDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            _inventory = session.player.inventory;
            _mainItemsInfo = BuildMainItemsInfo();
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
            sb.Append($"{Emojis.items[ItemType.Poison]} {_inventory.GetItemsCountByType(ItemType.Poison)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Tome]} {_inventory.GetItemsCountByType(ItemType.Tome)}");
            sb.Append(Emojis.bigSpace);
            sb.Append($"{Emojis.items[ItemType.Scroll]} {_inventory.GetItemsCountByType(ItemType.Scroll)}");

            return sb.ToString();
        }

        public override async Task SendAsync()
        {
            await ShowMainInfo();
        }

        public async Task ShowMainInfo()
        {
            await RemoveKeyboardFromLastMessage();
            _lastMessage = await messageSender.SendTextMessage(session.chatId, _mainItemsInfo, GetMultilineKeyboard());
        }

        public async Task ShowCategory(ItemType category)
        {
            if (_lastMessage == null)
                return;

            if (_compareData != null)
            {
                await ResendComparedItemMessage();
            }

            await RemoveKeyboardFromLastMessage();

            _browsedCategory = category;
            _browsedItems = GetBrowsedItems(category);

            if (_browsedItems.Length == 0)
            {
                var categoryLocalization = GetCategoryLocalization(category);
                var text = $"<b>{categoryLocalization}:</b> "
                    + Localization.Get(session, "dialog_inventory_has_not_items");
                _lastMessage = await messageSender.SendTextMessage(session.chatId, text);
                return;
            }

            _currentPage = 0;
            _pagesCount = _browsedItems.Length % browsedItemsOnPage > 0
                ? _browsedItems.Length / browsedItemsOnPage + 1
                : _browsedItems.Length / browsedItemsOnPage;

            await ShowItemsPage(asNewMessage: true);
        }

        private InventoryItem[] GetBrowsedItems(ItemType category)
        {
            if (category == ItemType.Tome)
            {
                var magicItems = new List<InventoryItem>();
                magicItems.AddRange(_inventory.GetItemsByType(ItemType.Tome));
                magicItems.AddRange(_inventory.GetItemsByType(ItemType.Scroll));
                return magicItems.ToArray();
            }
            else
            {
                return _inventory.GetItemsByType(category).ToArray();
            }
        }

        private async Task ShowItemsPage(bool asNewMessage)
        {
            var categoryLocalization = GetCategoryLocalization(_browsedCategory.Value);

            var text = new StringBuilder();
            text.AppendLine($"<b>{categoryLocalization}:</b> {_browsedItems.Length}");
            if (_pagesCount > 1)
            {
                text.AppendLine(string.Format(Localization.Get(session, "dialog_inventory_current_page"), _currentPage + 1, _pagesCount));
            }

            ClearButtons();
            int startIndex = _currentPage * browsedItemsOnPage;
            for (int i = startIndex; i < startIndex + browsedItemsOnPage && i < _browsedItems.Length; i++)
            {
                var item = _browsedItems[i];
                RegisterButton(item.GetFullName(session), async () => await OnItemClick(item));
            }

            RegisterButton(Emojis.menuItems[MenuItem.Inventory], async () => await OnClickCloseCategory());
            if (_pagesCount > 1)
            {
                if (_currentPage > 0)
                    RegisterButton("<<", async () => await OnClickPreviousPage());
                if (_currentPage < _pagesCount - 1)
                    RegisterButton(">>", async () => await OnClickNextPage());
            }

            if (asNewMessage)
            {
                _lastMessage = await messageSender.SendTextMessage(session.chatId, text.ToString(), GetItemsPageKeyboard());
            }
            else
            {
                _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text.ToString(), GetItemsPageKeyboard());
            }
        }

        private string GetCategoryLocalization(ItemType category)
        {
            if (category == ItemType.Tome)
            {
                return Localization.Get(session, $"menu_item_spells");
            }

            string stringCategory = category.ToString().ToLower();
            if (!stringCategory.EndsWith('s'))
            {
                stringCategory = stringCategory + 's';
            }
            return Localization.Get(session, $"menu_item_{stringCategory}");
        }

        private async Task OnItemClick(InventoryItem item)
        {
            if (_compareData != null)
            {
                await ShowComparingItems(item);
                return;
            }
            await ShowItemInspector(item);
        }

        private async Task OnClickCloseCategory()
        {
            await ShowMainInfo();
        }

        private async Task OnClickPreviousPage()
        {
            _currentPage--;
            await ShowItemsPage(asNewMessage: false);
        }

        private async Task OnClickNextPage()
        {
            _currentPage++;
            await ShowItemsPage(asNewMessage: false);
        }

        private InlineKeyboardMarkup GetItemsPageKeyboard()
        {
            if (_pagesCount < 2)
                return GetMultilineKeyboard();

            int lastRowButtons = _currentPage == _pagesCount - 1 || _currentPage == 0 ? 2 : 3;
            var paramers = new int[buttonsCount - lastRowButtons + 1];
            for (int i = 0; i < paramers.Length; i++)
            {
                paramers[i] = i < paramers.Length - 1 ? 1 : lastRowButtons;
            }
            return GetKeyboardWithRowSizes(paramers);
        }

        private async Task ShowItemInspector(InventoryItem item)
        {
            var text = item.GetView(session);

            ClearButtons();

            int firstRowButtons = 1;
            if (item.data.itemType.IsEquippable())
            {
                if (item.isEquipped)
                {
                    RegisterButton(Localization.Get(session, "menu_item_unequip_button"), null);
                }
                else
                {
                    RegisterButton(Localization.Get(session, "menu_item_equip_button"), null);
                }
                firstRowButtons++;
            }

            
            RegisterButton(Localization.Get(session, "menu_item_compare_button"),
                async () => await StartSelectItemForCompare(item),
                () => Localization.Get(session, "menu_item_compare_button_callback"));

            var categoryIcon = _browsedCategory == ItemType.Tome
                ? Emojis.menuItems[MenuItem.Spells]
                : Emojis.items[_browsedCategory.Value];

            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_to_list_button")} {categoryIcon}",
                async () => await ShowItemsPage(asNewMessage: false));

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetKeyboardWithRowSizes(firstRowButtons, 1));
        }

        private async Task StartSelectItemForCompare(InventoryItem item)
        {
            _compareData = new CompareData
            {
                comparedItem = item,
                categoryOnStartCompare = _browsedCategory,
                currentPageOnStartCompare = _currentPage,
                pagesCountOnStartCompare = _pagesCount,
                comparedItemMessage = await messageSender.EditMessageKeyboard(session.chatId, _lastMessage.MessageId, null)
            };

            await ShowItemsPage(asNewMessage: true);
        }

        private async Task ShowComparingItems(InventoryItem secondItem)
        {
            var text = secondItem.GetView(session);
            ClearButtons();

            RegisterButton(Localization.Get(session, "menu_item_compare_another_button"),
                async () => await ShowItemsPage(asNewMessage: false),
                () => Localization.Get(session, "menu_item_compare_button_callback"));

            RegisterButton(Localization.Get(session, "menu_item_compare_end_button"),
                async () => await EndComparison());

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private async Task EndComparison()
        {
            await messageSender.DeleteMessage(session.chatId, _compareData.Value.comparedItemMessage.MessageId);

            _browsedCategory = _compareData.Value.categoryOnStartCompare;
            _currentPage = _compareData.Value.currentPageOnStartCompare;
            _pagesCount = _compareData.Value.pagesCountOnStartCompare;
            var inspectedItem = _compareData.Value.comparedItem;
            _compareData = null;

            await ShowItemInspector(inspectedItem);
        }

        private async Task ResendComparedItemMessage()
        {
            var messageToResend = _compareData.Value.comparedItemMessage;
            if (_lastMessage != messageToResend)
            {
                await messageSender.DeleteMessage(session.chatId, _lastMessage.MessageId);
            }
            await messageSender.DeleteMessage(session.chatId, messageToResend.MessageId);
            _compareData = new CompareData
            {
                comparedItem = _compareData.Value.comparedItem,
                categoryOnStartCompare = _compareData.Value.categoryOnStartCompare,
                currentPageOnStartCompare = _compareData.Value.currentPageOnStartCompare,
                pagesCountOnStartCompare = _compareData.Value.pagesCountOnStartCompare,
                comparedItemMessage = await messageSender.SendTextMessage(session.chatId, _compareData.Value.comparedItem.GetView(session), silent: true)
            };
            _lastMessage = _compareData.Value.comparedItemMessage;
        }

        private async Task RemoveKeyboardFromLastMessage()
        {
            ClearButtons();
            if (_lastMessage?.ReplyMarkup != null)
            {
                await messageSender.EditMessageKeyboard(session.chatId, _lastMessage.MessageId, null);
            }
        }

        public override void OnDialogClose()
        {
        }

    }
}
