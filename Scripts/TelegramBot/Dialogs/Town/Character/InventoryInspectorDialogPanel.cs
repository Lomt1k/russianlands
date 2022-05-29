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
        

        public InventoryInspectorDialogPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
            _inventory = session.player.inventory;
            _mainItemsInfo = BuildMainItemsInfo();
        }

        private string BuildMainItemsInfo()
        {
            var sb = new StringBuilder();

            sb.Append($"{Emojis.items[ItemType.Sword]} {_inventory.GetItemsCountByType(ItemType.Sword)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Bow]} {_inventory.GetItemsCountByType(ItemType.Bow)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Stick]} {_inventory.GetItemsCountByType(ItemType.Stick)}");


            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Helmet]} {_inventory.GetItemsCountByType(ItemType.Helmet)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Armor]} {_inventory.GetItemsCountByType(ItemType.Armor)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Boots]} {_inventory.GetItemsCountByType(ItemType.Boots)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Shield]} {_inventory.GetItemsCountByType(ItemType.Shield)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Amulet]} {_inventory.GetItemsCountByType(ItemType.Amulet)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Ring]} {_inventory.GetItemsCountByType(ItemType.Ring)}");

            sb.AppendLine();
            sb.Append($"{Emojis.items[ItemType.Poison]} {_inventory.GetItemsCountByType(ItemType.Poison)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Tome]} {_inventory.GetItemsCountByType(ItemType.Tome)}");
            sb.Append(Emojis.space);
            sb.Append($"{Emojis.items[ItemType.Scroll]} {_inventory.GetItemsCountByType(ItemType.Scroll)}");

            return sb.ToString();
        }

        public override async Task SendAsync()
        {
            await ShowMainInfo();
        }

        public async Task ShowMainInfo()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine(Localization.Get(session, "dialog_inventory_header_all_items"));
            text.AppendLine();
            text.Append(_mainItemsInfo);

            await RemoveKeyboardFromLastMessage();
            _lastMessage = await messageSender.SendTextMessage(session.chatId, text.ToString(), GetMultilineKeyboard());
        }

        public async Task ShowCategory(ItemType category)
        {
            if (_lastMessage == null)
                return;

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
                magicItems.AddRange(_inventory.GetItemsByType(category));
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
                RegisterButton(item.GetFullName(session), async() => await OnItemClick(item));
            }

            RegisterButton(Emojis.menuItems[MenuItem.Inventory], async() => await OnClickCloseCategory());
            if (_pagesCount > 1)
            {
                if (_currentPage > 0)
                    RegisterButton("<<", async() => await OnClickPreviousPage());
                if (_currentPage < _pagesCount - 1)
                    RegisterButton(">>", async() => await OnClickNextPage());
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
            string stringCategory = category.ToString().ToLower();
            if (!stringCategory.EndsWith('s'))
            {
                stringCategory = stringCategory + 's';
            }
            return Localization.Get(session, $"menu_item_{stringCategory}");
        }

        private async Task OnItemClick(InventoryItem item)
        {
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

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text);
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
