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
        public ItemType categoryOnStartCompare;
        public int currentPageOnStartCompare;
        public int pagesCountOnStartCompare;
    }

    internal class InventoryInspectorDialogPanel : DialogPanelBase
    {
        private const int browsedItemsOnPage = 5;

        private readonly string _mainItemsInfo;

        private PlayerInventory _inventory;
        private Message? _lastMessage;

        private ItemType _browsedCategory;
        private InventoryItem[] _browsedItems;
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

            RegisterButton($"{Emojis.items[ItemType.Equipped]} {Localization.Get(session, "menu_item_equipped")}",
                () => ShowCategory(ItemType.Equipped));

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

            _browsedCategory = category;
            RefreshBrowsedItems();
            await ShowItemsPage(asNewMessage: true);
        }

        private void RefreshBrowsedItems()
        {
            switch (_browsedCategory)
            {
                case ItemType.Equipped:
                    _browsedItems = _inventory.equipped.allEquipped;
                    break;
                case ItemType.Tome:
                    var magicItems = new List<InventoryItem>();
                    magicItems.AddRange(_inventory.GetItemsByType(ItemType.Tome));
                    magicItems.AddRange(_inventory.GetItemsByType(ItemType.Scroll));
                    _browsedItems = magicItems.ToArray();
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
            await RemoveKeyboardFromLastMessage();
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

            RegisterButton(Emojis.menuItems[MenuItem.Inventory], () => OnClickCloseCategory());
            if (_pagesCount > 1)
            {
                text.AppendLine(string.Format(Localization.Get(session, "dialog_inventory_current_page"), _currentPage + 1, _pagesCount));
                if (_currentPage > 0)
                    RegisterButton("<<", () => OnClickPreviousPage());
                if (_currentPage < _pagesCount - 1)
                    RegisterButton(">>", () => OnClickNextPage());
            }

            _lastMessage = asNewMessage
                ? await messageSender.SendTextMessage(session.chatId, text.ToString(), GetItemsPageKeyboard())
                : await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text.ToString(), GetItemsPageKeyboard());
        }

        private string GetCategoryLocalization(ItemType category)
        {
            switch (category)
            {
                case ItemType.Equipped:
                    return Localization.Get(session, $"menu_item_equipped");
                case ItemType.Tome:
                    return Localization.Get(session, $"menu_item_spells");
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
                    RegisterButton(Localization.Get(session, "menu_item_unequip_button"), () => UnequipItem(item));
                }
                else
                {
                    RegisterButton(Localization.Get(session, "menu_item_equip_button"), () => StartEquipLogic(item));
                }
                firstRowButtons++;
            }

            RegisterButton(Localization.Get(session, "menu_item_compare_button"),
                () => StartSelectItemForCompare(item),
                () => Localization.Get(session, "menu_item_compare_button_callback"));

            var categoryIcon = _browsedCategory == ItemType.Tome
                ? Emojis.menuItems[MenuItem.Spells]
                : Emojis.items[_browsedCategory];

            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_to_list_button")} {categoryIcon}",
                () => ShowItemsPage(asNewMessage: false));

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetKeyboardWithRowSizes(firstRowButtons, 1));
        }

        private async Task StartEquipLogic(InventoryItem item)
        {
            var profileLevel = session.profile.data.level;
            var requiredLevel = item.data.requiredLevel;
            if (profileLevel < requiredLevel)
            {
                var messageText = $"<b>{item.GetFullName(session)}</b>\n\n"
                    + string.Format(Localization.Get(session, "dialog_inventory_required_level"), requiredLevel) + $" {Emojis.smiles[Smile.Sad]}";
                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_ok_button"), () => ShowItemInspector(item));
                _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, messageText, GetOneLineKeyboard());
                return;
            }

            if (item.data.itemType.IsMultiSlot())
            {
                await SelectSlotForEquip(item);
                return;
            }

            await EquipSingleSlot(item);
        }

        private async Task SelectSlotForEquip(InventoryItem item)
        {
            var type = item.data.itemType;
            var slotsCount = type.GetSlotsCount();
            var category = GetCategoryLocalization(type);
            var text = string.Format(Localization.Get(session, "dialog_inventory_select_slot_for_equip"), category, slotsCount, item.GetFullName(session));

            ClearButtons();
            for (int i = 0; i < slotsCount; i++)
            {
                int slotId = i;
                var equippedItem = _inventory.equipped[type, slotId];
                var buttonText = equippedItem != null 
                    ? equippedItem.GetFullName(session) 
                    : $"{Emojis.items[type]} {Localization.Get(session, "menu_item_empty_slot_button")}";
                RegisterButton(buttonText, () => EquipMultiSlot(item, slotId));
            }
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", () => ShowItemInspector(item));

            _lastMessage = await messageSender.EditTextMessage(session.chatId, _lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private async Task EquipSingleSlot(InventoryItem item)
        {
            _inventory.EquipSingleSlot(item);
            RefreshBrowsedItems();
            await ShowItemInspector(item);
        }

        private async Task EquipMultiSlot(InventoryItem item, int slotId)
        {
            _inventory.EquipMultiSlot(item, slotId);
            RefreshBrowsedItems();
            await ShowItemInspector(item);
        }

        private async Task UnequipItem(InventoryItem item)
        {
            _inventory.Unequip(item);
            RefreshBrowsedItems();
            await ShowItemInspector(item);
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
            _lastMessage = _compareData.Value.comparedItemMessage;

            await ShowItemsPage(asNewMessage: true);
        }

        private async Task ShowComparingItems(InventoryItem secondItem)
        {
            var text = secondItem.GetView(session);
            ClearButtons();

            RegisterButton(Localization.Get(session, "menu_item_compare_another_button"), () => ShowItemsPage(asNewMessage: false),
                () => Localization.Get(session, "menu_item_compare_button_callback"));

            RegisterButton(Localization.Get(session, "menu_item_compare_end_button"), () => EndComparison());

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

            RefreshBrowsedItems();
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
