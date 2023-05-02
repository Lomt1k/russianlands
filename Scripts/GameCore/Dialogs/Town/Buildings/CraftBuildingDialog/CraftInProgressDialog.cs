using System;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Buildings.CraftBuildingDialog;

public class CraftInProgressDialog : DialogBase
{
    private readonly CraftBuildingBase _building;

    private ProfileBuildingsData buildingsData => session.profile.buildingsData;
    public CraftInProgressDialog(GameSession session, CraftBuildingBase building) : base(session)
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

        sb.AppendLine();
        var timeSpan = _building.GetEndCraftTime(buildingsData) - DateTime.UtcNow;
        var productionView = Localization.Get(session, "dialog_craft_progress", timeSpan.GetView(session));
        sb.AppendLine(Emojis.ElementSmallBlack + productionView);

        ClearButtons();
        var diamondsForBoost = GetBoostPriceInDiamonds();
        var priceView = ResourceId.Diamond.GetEmoji().ToString() + diamondsForBoost;
        var buttonText = Localization.Get(session, "menu_item_boost_button", priceView);
        RegisterButton(buttonText, TryBoostCraftForDiamonds);
        RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuilding(_building));
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    public async Task TryBoostCraftForDiamonds()
    {
        if (IsCraftCanBeFinished())
        {
            var message = Emojis.ElementClock + Localization.Get(session, "dialog_craft_boost_expired");
            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                () => new CraftCanCollectItemDialog(session, _building).Start());
            await SendDialogMessage(message, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var requiredDiamonds = GetBoostPriceInDiamonds();
        var playerResources = session.player.resources;
        var successsPurchase = playerResources.TryPurchase(requiredDiamonds, out var notEnoughDiamonds);
        if (successsPurchase)
        {
            _building.BoostCraft(buildingsData);

            var sb = new StringBuilder();
            sb.AppendLine(Emojis.ButtonCraft + Localization.Get(session, "dialog_craft_boosted"));
            if (requiredDiamonds.amount > 0)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                sb.AppendLine(requiredDiamonds.GetLocalizedView(session));
            }

            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"),
                () => new CraftCanCollectItemDialog(session, _building).TryToGetItem());

            await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
            return;
        }

        ClearButtons();
        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
            () => new ShopDialog(session).Start());
        RegisterBackButton(() => new CraftInProgressDialog(session, _building).Start());

        await SendDialogMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    public ResourceData GetBoostPriceInDiamonds()
    {
        if (IsCraftCanBeFinished())
        {
            return new ResourceData(ResourceId.Diamond, 0);
        }

        var endCraftTime = _building.GetEndCraftTime(buildingsData);
        var seconds = (int)(endCraftTime - DateTime.UtcNow).TotalSeconds;
        return ResourceHelper.CalculateCraftBoostPriceInDiamonds(seconds);
    }

    public bool IsCraftCanBeFinished()
    {
        return _building.IsCraftStarted(buildingsData) && _building.IsCraftCanBeFinished(buildingsData);
    }
}
