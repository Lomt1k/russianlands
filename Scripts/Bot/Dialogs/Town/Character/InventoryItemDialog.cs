using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Resources;

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
            await ShowItemInspector()
                .ConfigureAwait(false);
        }

        private async Task ShowItemInspector()
        {
            var sb = new StringBuilder();
            sb.Append(_item.GetView(session));

            ClearButtons();
            if (_item.isEquipped)
            {
                RegisterButton(Localization.Get(session, "menu_item_unequip_button"), () => UnequipItem());
            }
            else
            {
                RegisterButton(Localization.Get(session, "menu_item_equip_button"), () => StartEquipLogic());
            }

            RegisterButton(Localization.Get(session, "menu_item_compare_button"),
                () => StartSelectItemForCompare());

            RegisterButton($"{Emojis.elements[Element.Bin]} {Localization.Get(session, "menu_item_break_apart_button")}",
                () => TryBreakApartItem());

            var categoryIcon = Emojis.items[_browsedCategory];
            RegisterButton($"{Emojis.elements[Element.Back]} {_browsedCategory.GetCategoryLocalization(session)} {categoryIcon}",
                () => new InventoryDialog(session).ShowCategory(_browsedCategory, _browsedPage));
            RegisterDoubleBackButton($"{Localization.Get(session, "menu_item_inventory")} {Emojis.menuItems[MenuItem.Inventory]}",
                () => new InventoryDialog(session).Start());

            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task StartEquipLogic()
        {
            var profileLevel = session.profile.data.level;
            var requiredLevel = _item.data.requiredLevel;
            if (profileLevel < requiredLevel)
            {
                var messageText = $"<b>{_item.GetFullName(session)}</b>\n\n"
                    + Localization.Get(session, "dialog_inventory_required_level", requiredLevel, Emojis.smiles[Smile.Sad]);
                await SendMessageWithBackButton(messageText)
                    .ConfigureAwait(false);
                return;
            }

            if (_item.data.itemType.IsMultiSlot())
            {
                await SelectSlotForEquip()
                    .ConfigureAwait(false);
                return;
            }

            await EquipSingleSlot()
                .ConfigureAwait(false);
        }

        private async Task SelectSlotForEquip()
        {
            var type = _item.data.itemType;
            var slotsCount = type.GetSlotsCount();
            var category = GetCategoryLocalization(type);
            var text = Localization.Get(session, "dialog_inventory_select_slot_for_equip", category, slotsCount, _item.GetFullName(session));

            ClearButtons();
            for (int i = 0; i < slotsCount; i++)
            {
                int slotId = i;
                var equippedItem = inventory.equipped[type, slotId];
                var buttonText = equippedItem != null
                    ? equippedItem.GetFullName(session)
                    : $"{Emojis.items[type]} {Localization.Get(session, "menu_item_empty_slot_button")}";
                RegisterButton(buttonText, () => EquipMultiSlot(slotId));
            }
            RegisterBackButton(() => ShowItemInspector());

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

        private async Task EquipSingleSlot()
        {
            inventory.EquipSingleSlot(_item);
            await ShowItemInspector()
                .ConfigureAwait(false);
        }

        private async Task EquipMultiSlot(int slotId)
        {
            inventory.EquipMultiSlot(_item, slotId);
            await ShowItemInspector()
                .ConfigureAwait(false);
        }

        private async Task UnequipItem()
        {
            inventory.Unequip(_item);
            await ShowItemInspector()
                .ConfigureAwait(false);
        }

        private async Task StartSelectItemForCompare()
        {
            var compareData = new CompareData
            {
                comparedItem = _item,
                categoryOnStartCompare = _browsedCategory,
                currentPageOnStartCompare = _browsedPage,
            };

            await new InventoryDialog(session).ShowCategory(_browsedCategory, newCompareData: compareData)
                .ConfigureAwait(false);
        }

        private async Task TryBreakApartItem()
        {
            if (_item.isEquipped)
            {
                await SendMessageWithBackButton(Localization.Get(session, "dialog_inventory_break_apart_equipped"))
                    .ConfigureAwait(false);
                return;
            }
            if (inventory.GetItemsCountByType(_item.data.itemType) < 2)
            {
                var message = Localization.Get(session, "dialog_inventory_break_apart_empty_category")
                    + $"\n{Emojis.items[_browsedCategory]} <b>{_browsedCategory.GetCategoryLocalization(session)}</b>";
                await SendMessageWithBackButton(message)
                    .ConfigureAwait(false);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("<b>" + _item.GetFullName(session) + "</b>");
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_inventory_break_apart_confirm"));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_you_will_get"));
            var rewardResources = _item.CalculateResourcesForBreakApart();
            sb.Append(ResourceHelper.GetResourcesView(session, rewardResources));

            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Bin]} {Localization.Get(session, "menu_item_break_apart_button")}",
                () => ForceBreakApart());
            RegisterBackButton(() => ShowItemInspector());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        private async Task ForceBreakApart()
        {
            var rewardResources = _item.CalculateResourcesForBreakApart();
            session.player.resources.ForceAdd(rewardResources);
            inventory.RemoveItem(_item);

            await new InventoryDialog(session).ShowCategory(_browsedCategory, _browsedPage)
                .ConfigureAwait(false);
        }

        private async Task SendMessageWithBackButton(string text)
        {
            ClearButtons();
            RegisterBackButton(() => ShowItemInspector());
            await SendDialogMessage(text, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
