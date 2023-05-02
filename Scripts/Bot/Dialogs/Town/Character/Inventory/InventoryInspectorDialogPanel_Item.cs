using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character.Inventory
{
    public partial class InventoryInspectorDialogPanel : DialogPanelBase
    {
        private InventoryItem? _browsedItem;

        private PlayerInventory inventory => session.player.inventory;

        private async Task ShowItemInspector()
        {
            var sb = new StringBuilder();
            sb.Append(_browsedItem.GetView(session));

            ClearButtons();
            if (_browsedItem.isEquipped)
            {
                RegisterButton(Localization.Get(session, "menu_item_unequip_button"), UnequipItem);
            }
            else
            {
                RegisterButton(Localization.Get(session, "menu_item_equip_button"), StartEquipLogic);
            }

            RegisterButton(Localization.Get(session, "menu_item_compare_button"),
                StartSelectItemForCompare,
                () => Localization.Get(session, "dialog_inventory_start_compare_query"));

            RegisterButton(Emojis.ElementBin + Localization.Get(session, "menu_item_break_apart_button"), TryBreakApartItem);

            RegisterBackButton(_browsedCategory.GetCategoryLocalization(session) + _browsedCategory.GetEmoji(), ShowItemsPage);
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_inventory") + Emojis.ButtonInventory, ShowCategories);

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
        }

        private async Task StartEquipLogic()
        {
            var profileLevel = session.profile.data.level;
            var requiredLevel = _browsedItem.data.requiredLevel;
            if (profileLevel < requiredLevel)
            {
                var messageText = _browsedItem.GetFullName(session).Bold() + "\n\n"
                    + Localization.Get(session, "dialog_inventory_required_level", requiredLevel, Emojis.SmileSad);
                await SendMessageWithBackButton(messageText).FastAwait();
                return;
            }

            if (_browsedItem.data.itemType.IsMultiSlot())
            {
                await SelectSlotForEquip().FastAwait();
                return;
            }

            await EquipSingleSlot().FastAwait();
        }

        private async Task SelectSlotForEquip()
        {
            var type = _browsedItem.data.itemType;
            var slotsCount = type.GetSlotsCount();
            var category = GetCategoryLocalization(type);
            var text = Localization.Get(session, "dialog_inventory_select_slot_for_equip", category, slotsCount, _browsedItem.GetFullName(session));

            ClearButtons();
            for (int i = 0; i < slotsCount; i++)
            {
                int slotId = i;
                var equippedItem = inventory.equipped[type, slotId];
                var buttonText = equippedItem != null
                    ? equippedItem.GetFullName(session).RemoveHtmlTags()
                    : type.GetEmoji() + Localization.Get(session, "menu_item_empty_slot_button");
                RegisterButton(buttonText, () => EquipMultiSlot(slotId));
            }
            RegisterBackButton(ShowItemInspector);

            await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
        }

        private async Task EquipSingleSlot()
        {
            inventory.EquipSingleSlot(_browsedItem);
            RefreshBrowsedItemsIfEquippedCategory();

            await ShowItemInspector().FastAwait();
        }

        private async Task EquipMultiSlot(int slotId)
        {
            inventory.EquipMultiSlot(_browsedItem, slotId);
            RefreshBrowsedItemsIfEquippedCategory();

            await ShowItemInspector().FastAwait();
        }

        private async Task UnequipItem()
        {
            inventory.Unequip(_browsedItem);
            RefreshBrowsedItemsIfEquippedCategory();

            await ShowItemInspector().FastAwait();
        }

        // Если мы убираем / надеваем предмет, находясь в категории "экипированного" - список экипированных предметов должен обновиться
        private void RefreshBrowsedItemsIfEquippedCategory()
        {
            if (_browsedCategory == ItemType.Equipped)
            {
                RefreshBrowsedItems();
            }
        }

        private async Task StartSelectItemForCompare()
        {
            _compareData = new CompareData
            {
                comparedItem = _browsedItem,
                categoryOnStartCompare = _browsedCategory,
                currentPageOnStartCompare = _currentPage,
                pagesCountOnStartCompare = _pagesCount,
            };

            await ShowCategory(_browsedCategory).FastAwait();
        }

        private async Task TryBreakApartItem()
        {
            if (_browsedItem.isEquipped)
            {
                await SendMessageWithBackButton(Localization.Get(session, "dialog_inventory_break_apart_equipped")).FastAwait();
                return;
            }
            if (inventory.GetItemsCountByType(_browsedItem.data.itemType) < 2)
            {
                var message = Localization.Get(session, "dialog_inventory_break_apart_empty_category")
                    + _browsedCategory.GetEmoji() + _browsedCategory.GetCategoryLocalization(session).Bold();
                await SendMessageWithBackButton(message).FastAwait();
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(_browsedItem.GetFullName(session).Bold());
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_inventory_break_apart_confirm"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_you_will_get"));
            var rewardResources = _browsedItem.CalculateResourcesForBreakApart();
            sb.Append(rewardResources.GetLocalizedView(session));

            ClearButtons();
            RegisterButton(Emojis.ElementBin + Localization.Get(session, "menu_item_break_apart_button"), ForceBreakApart);
            RegisterBackButton(ShowItemInspector);

            await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private async Task ForceBreakApart()
        {
            var rewardResources = _browsedItem.CalculateResourcesForBreakApart();
            session.player.resources.ForceAdd(rewardResources);
            inventory.RemoveItem(_browsedItem);
            _browsedItem = null;

            await ShowCategory(_browsedCategory, _currentPage).FastAwait();
        }

        private async Task SendMessageWithBackButton(string text)
        {
            ClearButtons();
            RegisterBackButton(ShowItemInspector);
            await SendPanelMessage(text, GetOneLineKeyboard()).FastAwait();
        }

        #region Comparison

        private async Task ShowItemInspectorWithComparison()
        {
            ClearButtons();

            RegisterButton(Localization.Get(session, "menu_item_compare_another_button"),() => SelectAnotherItemToCompare());
            RegisterButton(Localization.Get(session, "menu_item_compare_end_button"), () => EndComparison());

            var sb = new StringBuilder();
            sb.AppendLine(_compareData.Value.comparedItem.GetView(session));
            sb.AppendLine();
            sb.AppendLine("========================");
            sb.AppendLine();
            sb.AppendLine(_browsedItem.GetView(session));

            await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
        }

        private async Task SelectAnotherItemToCompare()
        {
            await ShowItemsPage();
        }

        private async Task EndComparison()
        {
            _browsedItem = _compareData.Value.comparedItem;
            _currentPage = _compareData.Value.currentPageOnStartCompare;
            _browsedCategory = _compareData.Value.categoryOnStartCompare;
            _pagesCount = _compareData.Value.pagesCountOnStartCompare;
            _compareData = null;
            RefreshBrowsedItems();
            await ShowItemInspector();
        }

        #endregion

    }
}
