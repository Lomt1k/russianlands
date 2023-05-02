using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using MarkOne.Scripts.GameCore.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Quests.QuestStages;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Character.Inventory;

public struct CompareData
{
    public InventoryItem comparedItem;
    public ItemType categoryOnStartCompare;
    public int currentPageOnStartCompare;
    public int pagesCountOnStartCompare;
}

public partial class InventoryInspectorDialogPanel : DialogPanelBase
{
    private const int browsedItemsOnPage = 8;

    public CompareData? _compareData;

    private readonly PlayerInventory _inventory;
    private ItemType _browsedCategory;
    private InventoryItem[] _browsedItems;
    private int _currentPage;
    private int _pagesCount;

    public InventoryInspectorDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
        _inventory = session.player.inventory;
    }

    private string BuildMainItemsInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(session, "dialog_inventory_header_total_items", _inventory.itemsCount, _inventory.inventorySize));

        sb.AppendLine();
        sb.Append(ItemType.Sword.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Sword).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Bow.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Bow).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Stick.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Stick).ToString());


        sb.AppendLine();
        sb.Append(ItemType.Helmet.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Helmet).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Armor.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Armor).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Armor.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Armor).ToString());

        sb.AppendLine();
        sb.Append(ItemType.Shield.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Shield).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Ring.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Ring).ToString());
        sb.Append(Emojis.bigSpace);
        sb.Append(ItemType.Amulet.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Amulet).ToString());

        sb.AppendLine();
        sb.Append(ItemType.Scroll.GetEmoji() + _inventory.GetItemsCountByType(ItemType.Scroll).ToString());

        return sb.ToString();
    }

    public override async Task Start()
    {
        await ShowCategories().FastAwait();
    }

    private async Task ShowCategories()
    {
        var tooltip = session.tooltipController.TryGetTooltip(this);

        ClearButtons();
        RegisterCategoryButton(ItemType.Sword, tooltip, 0);
        RegisterCategoryButton(ItemType.Bow, tooltip, 1);
        RegisterCategoryButton(ItemType.Stick, tooltip, 2);
        RegisterCategoryButton(ItemType.Helmet, tooltip, 3);
        RegisterCategoryButton(ItemType.Armor, tooltip, 4);
        RegisterCategoryButton(ItemType.Boots, tooltip, 5);
        RegisterCategoryButton(ItemType.Shield, tooltip, 6);
        RegisterCategoryButton(ItemType.Ring, tooltip, 7);
        RegisterCategoryButton(ItemType.Amulet, tooltip, 8);
        RegisterCategoryButton(ItemType.Scroll, tooltip, 9);

        RegisterButton(Emojis.ItemEquipped + Localization.Get(session, "menu_item_equipped"),
            () => ShowCategory(ItemType.Equipped));

        var sb = new StringBuilder();
        sb.Append(BuildMainItemsInfo());
        var dialogHasTooltip = TryAppendTooltip(sb, tooltip);

        await SendPanelMessage(sb, GetKeyboardWithFixedRowSize(3)).FastAwait();
    }

    private void RegisterCategoryButton(ItemType itemType, Tooltip? tooltip, int buttonId)
    {
        var inventory = session.player.inventory;
        var hasTooltip = tooltip != null;
        var isTooltipButton = hasTooltip && tooltip.buttonId == buttonId;

        var prefix = isTooltipButton ? string.Empty : itemType.GetEmoji().ToString() + ' ';
        var postfix = !hasTooltip && inventory.HasNewInCategory(itemType)
            ? Emojis.ElementWarningRed.ToString()
            : string.Empty;

        var text = prefix + itemType.GetCategoryLocalization(session) + postfix;
        RegisterButton(text, () => ShowCategory(itemType));
    }

    public async Task ShowCategory(ItemType category, int itemsPage = 0)
    {
        _browsedCategory = category;
        RefreshBrowsedItems();
        _currentPage = itemsPage < _pagesCount ? itemsPage : _pagesCount - 1;
        await ShowItemsPage().FastAwait();
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
            ? (_browsedItems.Length / browsedItemsOnPage) + 1
            : _browsedItems.Length / browsedItemsOnPage;
    }

    private async Task ShowItemsPage()
    {
        ClearButtons();
        var categoryLocalization = GetCategoryLocalization(_browsedCategory) + ": ";
        var hasTooltip = session.tooltipController.HasTooltipToAppend(this);
        var text = new StringBuilder();
        text.Append(categoryLocalization.Bold());

        if (_pagesCount == 0)
        {
            text.Append(Localization.Get(session, "dialog_inventory_has_not_items"));
        }
        else
        {
            text.AppendLine(_browsedItems.Length.ToString());
            var startIndex = _currentPage * browsedItemsOnPage;
            for (var i = startIndex; i < startIndex + browsedItemsOnPage && i < _browsedItems.Length; i++)
            {
                var item = _browsedItems[i];
                var prefix = item.isEquipped ? Emojis.ItemEquipped : Emojis.Empty;
                var itemButton = prefix + item.GetFullName(session).RemoveHtmlTags()
                    + (item.isNew && !hasTooltip ? Emojis.ElementWarningRed.ToString() : string.Empty);
                RegisterButton(itemButton, () => OnItemClick(item));
            }
        }

        RegisterBackButton(ShowCategories);
        if (_pagesCount > 1)
        {
            text.AppendLine(Localization.Get(session, "dialog_inventory_current_page", _currentPage + 1, _pagesCount));
            if (_currentPage > 0)
            {
                RegisterButton("<<", OnClickPreviousPage);
            }
            else
            {
                RegisterButton(".", null);
            }

            if (_currentPage < _pagesCount - 1)
            {
                RegisterButton(">>", OnClickNextPage);
            }
            else
            {
                RegisterButton(".", null);
            }
        }

        TryAppendTooltip(text);
        await SendPanelMessage(text, GetItemsPageKeyboard()).FastAwait();
    }

    private string GetCategoryLocalization(ItemType category)
    {
        switch (category)
        {
            case ItemType.Equipped:
                return Localization.Get(session, $"menu_item_equipped");
            default:
                var stringCategory = category.ToString().ToLower();
                if (!stringCategory.EndsWith('s'))
                {
                    stringCategory = stringCategory + 's';
                }
                return Localization.Get(session, $"menu_item_{stringCategory}");
        }
    }

    private async Task OnItemClick(InventoryItem item)
    {
        _browsedItem = item;
        MarkItemAsViewed(item);

        if (_compareData != null)
        {
            await ShowItemInspectorWithComparison().FastAwait();
            return;
        }


        await ShowItemInspector().FastAwait();
    }

    private void MarkItemAsViewed(InventoryItem item)
    {
        if (item.state == ItemState.IsNewAndNotEquipped)
        {
            item.state = ItemState.IsNotEquipped;
            session.player.inventory.UpdateHasNewItemsState();
        }
    }

    private async Task OnClickPreviousPage()
    {
        _currentPage--;
        await ShowItemsPage().FastAwait();
    }

    private async Task OnClickNextPage()
    {
        _currentPage++;
        await ShowItemsPage().FastAwait();
    }

    private InlineKeyboardMarkup GetItemsPageKeyboard()
    {
        if (_pagesCount < 2)
            return GetMultilineKeyboard();

        var parameters = new int[buttonsCount - 2];
        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = i < parameters.Length - 1 ? 1 : 3;
        }
        return GetKeyboardWithRowSizes(parameters);
    }

}
