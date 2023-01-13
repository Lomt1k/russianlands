using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings.CraftBuildingDialog
{
    public class CraftCanCollectItemDialog : DialogBase
    {
        private CraftBuildingBase _building;

        private ProfileBuildingsData buildingsData => session.profile.buildingsData;

        public CraftCanCollectItemDialog(GameSession session, CraftBuildingBase building) : base(session)
        {
            _building = building;
        }

        public override async Task Start()
        {
            var itemType = _building.GetCurrentCraftItemType(buildingsData);
            var rarity = _building.GetCurrentCraftItemRarity(buildingsData);

            var sb = new StringBuilder();
            sb.AppendLine($"<b>{Emojis.items[itemType]} {itemType.GetLocalization(session)}</b>");
            sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
            var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
            sb.AppendLine(string.Format(Localization.Get(session, "level"), craftItemLevels));

            sb.AppendLine();
            sb.AppendLine($"{Emojis.menuItems[MenuItem.Craft]} {Localization.Get(session, "dialog_craft_completed")}");

            ClearButtons();
            var getButton = $"{Localization.Get(session, "menu_item_get_button")} {Emojis.items[itemType]}";
            RegisterButton(getButton, () => TryToGetItem());
            RegisterBackButton(() => new BuildingInfoDialog(session, _building).Start());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        public async Task TryToGetItem()
        {
            var inventory = session.player.inventory;
            if (inventory.isFull)
            {
                var totalItems = string.Format(Localization.Get(session, "dialog_inventory_header_total_items"), inventory.itemsCount, inventory.inventorySize);
                var text = string.Format(Localization.Get(session, "dialog_inventory_is_full"), totalItems);

                ClearButtons();
                RegisterButton($"{Emojis.menuItems[MenuItem.Inventory]} {Localization.Get(session, "menu_item_inventory")}",
                    () => new InventoryDialog(session).Start());
                RegisterBackButton(() => Start());

                await SendDialogMessage(text, GetMultilineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            var item = _building.GetCraftItemAndResetCraft(buildingsData);
            inventory.ForceAddItem(item);

            var sb = new StringBuilder();
            sb.AppendLine(item.GetView(session));
            sb.AppendLine();
            sb.Append(Localization.Get(session, "dialog_inventory_item_added_state"));

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                () => new BuildingInfoDialog(session, _building).Start());

            await SendDialogMessage(sb, GetOneLineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
