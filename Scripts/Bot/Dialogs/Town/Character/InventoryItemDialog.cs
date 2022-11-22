using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Character
{
    public class InventoryItemDialog : DialogBase
    {
        private InventoryItem _item;
        private ItemType _browsedCategory;
        private int _browsedPage;

        private PlayerInventory inventory => session.player.inventory;

        public InventoryItemDialog(GameSession _session, InventoryItem item, ItemType browsedCategory, int browsedPage) : base(_session)
        {
            _item = item;
            _browsedCategory = browsedCategory;
            _browsedPage = browsedPage;
        }

        public override async Task Start()
        {
            await ShowItemInspector(_item)
                .ConfigureAwait(false);
        }

        private async Task ShowItemInspector(InventoryItem item)
        {
            var sb = new StringBuilder();
            sb.Append(item.GetView(session));

            ClearButtons();
            if (item.isEquipped)
            {
                RegisterButton(Localization.Get(session, "menu_item_unequip_button"), () => UnequipItem(item));
            }
            else
            {
                RegisterButton(Localization.Get(session, "menu_item_equip_button"), () => StartEquipLogic(item));
            }

            RegisterButton(Localization.Get(session, "menu_item_compare_button"),
                () => StartSelectItemForCompare(item));

            var categoryIcon = Emojis.items[_browsedCategory];
            RegisterButton($"{Emojis.elements[Element.Back]} {_browsedCategory.GetCategoryLocalization(session)} {categoryIcon}",
                () => new InventoryDialog(session).ShowCategory(_browsedCategory, _browsedPage));
            RegisterButton($"{Emojis.elements[Element.FullBack]} {Localization.Get(session, "menu_item_inventory")} {Emojis.menuItems[MenuItem.Inventory]}",
                () => new InventoryDialog(session).Start());

            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
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
                await SendDialogMessage(messageText, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            if (item.data.itemType.IsMultiSlot())
            {
                await SelectSlotForEquip(item)
                    .ConfigureAwait(false);
                return;
            }

            await EquipSingleSlot(item)
                .ConfigureAwait(false);
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
                var equippedItem = inventory.equipped[type, slotId];
                var buttonText = equippedItem != null
                    ? equippedItem.GetFullName(session)
                    : $"{Emojis.items[type]} {Localization.Get(session, "menu_item_empty_slot_button")}";
                RegisterButton(buttonText, () => EquipMultiSlot(item, slotId));
            }
            RegisterBackButton(() => ShowItemInspector(item));

            await SendDialogMessage(text, GetMultilineKeyboard())
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

        private async Task EquipSingleSlot(InventoryItem item)
        {
            inventory.EquipSingleSlot(item);
            await ShowItemInspector(item)
                .ConfigureAwait(false);
        }

        private async Task EquipMultiSlot(InventoryItem item, int slotId)
        {
            inventory.EquipMultiSlot(item, slotId);
            await ShowItemInspector(item)
                .ConfigureAwait(false);
        }

        private async Task UnequipItem(InventoryItem item)
        {
            inventory.Unequip(item);
            await ShowItemInspector(item)
                .ConfigureAwait(false);
        }

        private async Task StartSelectItemForCompare(InventoryItem item)
        {
            var compareData = new CompareData
            {
                comparedItem = item,
                categoryOnStartCompare = _browsedCategory,
                currentPageOnStartCompare = _browsedPage,
            };

            await new InventoryDialog(session).ShowCategory(_browsedCategory, newCompareData: compareData)
                .ConfigureAwait(false);
        }

    }
}
