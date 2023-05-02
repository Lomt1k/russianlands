using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Character.Inventory;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Buildings.CraftBuildingDialog;

public class CraftCanCollectItemDialog : DialogBase
{
    private readonly CraftBuildingBase _building;

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
        sb.AppendLine(itemType.GetEmoji() + itemType.GetLocalization(session).Bold());
        sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
        var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
        sb.AppendLine(Localization.Get(session, "level", craftItemLevels));

        ClearButtons();
        var getButton = Localization.Get(session, "menu_item_get_button") + itemType.GetEmoji();
        RegisterButton(getButton, TryToGetItem);
        RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuilding(_building));
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    public async Task TryToGetItem()
    {
        var inventory = session.player.inventory;
        if (inventory.isFull)
        {
            var totalItems = Localization.Get(session, "dialog_inventory_header_total_items", inventory.itemsCount, inventory.inventorySize);
            var text = Localization.Get(session, "dialog_inventory_is_full", totalItems);

            ClearButtons();
            RegisterButton(Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory"),
                () => new InventoryDialog(session).Start());
            RegisterBackButton(Start);

            await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
            return;
        }

        var item = _building.GetCraftItemAndResetCraft(buildingsData);
        inventory.ForceAddItem(item);

        var sb = new StringBuilder();
        sb.AppendLine(item.GetView(session));
        sb.AppendLine();
        sb.Append(Localization.Get(session, "dialog_inventory_item_added_state"));

        ClearButtons();
        RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => new BuildingsDialog(session).StartWithShowBuilding(_building));

        await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
    }

}
