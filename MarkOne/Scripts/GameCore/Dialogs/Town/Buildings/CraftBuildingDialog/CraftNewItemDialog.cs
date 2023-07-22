using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using FastTelegramBot.DataTypes.Keyboards;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Buildings.CraftBuildingDialog;

public class CraftNewItemDialog : DialogBase
{
    private readonly CraftBuildingBase _building;

    private ProfileBuildingsData buildingsData => session.profile.buildingsData;

    public CraftNewItemDialog(GameSession session, CraftBuildingBase building) : base(session)
    {
        _building = building;
    }

    public override async Task Start()
    {
        var sb = new StringBuilder();
        sb.AppendLine(session.player.resources.GetCraftResourcesView());
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_craft_select_item_type"));

        ClearButtons();
        foreach (var itemType in _building.craftCategories)
        {
            RegisterButton(itemType.GetEmoji() + itemType.GetLocalization(session), () => StartSelectRarity(itemType));
        }
        RegisterBackButton(() => new BuildingsDialog(session).StartWithShowBuilding(_building));
        RegisterTownButton(isDoubleBack: true);

        await SendDialogMessage(sb, GetSpecialKeyboard()).FastAwait();
    }

    private ReplyKeyboardMarkup GetSpecialKeyboard()
    {
        return _building.craftCategories.Count switch
        {
            1 => GetMultilineKeyboardWithDoubleBack(),
            2 => GetKeyboardWithRowSizes(2, 2),
            3 => GetKeyboardWithRowSizes(3, 2),
            4 => GetKeyboardWithRowSizes(2, 2, 2),
            5 => GetKeyboardWithRowSizes(3, 2, 2),
            6 => GetKeyboardWithRowSizes(3, 3, 2),
            _ => GetMultilineKeyboardWithDoubleBack()
        };
    }

    private async Task StartSelectRarity(ItemType itemType)
    {
        var sb = new StringBuilder();
        sb.AppendLine(itemType.GetEmoji() + itemType.GetLocalization(session).Bold());
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "dialog_craft_select_item_rarity"));

        ClearButtons();
        RegisterRarityButton(itemType, Rarity.Rare);
        RegisterRarityButton(itemType, Rarity.Epic);
        RegisterRarityButton(itemType, Rarity.Legendary);
        RegisterBackButton(Start);

        await SendDialogMessage(sb, GetKeyboardWithRowSizes(3, 1)).FastAwait();
    }

    private void RegisterRarityButton(ItemType itemType, Rarity rarity)
    {
        RegisterButton(rarity.GetView(session), () => ShowCraftPrice(itemType, rarity));
    }

    private async Task ShowCraftPrice(ItemType itemType, Rarity rarity)
    {
        var sb = new StringBuilder();
        sb.AppendLine(itemType.GetEmoji() + itemType.GetLocalization(session).Bold());
        sb.AppendLine($"<pre>{rarity.GetView(session)}</pre>");

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "item_view_possible_requirments"));
        var craftItemLevels = _building.GetCurrentCraftLevels(buildingsData);
        sb.AppendLine(Localization.Get(session, "level", craftItemLevels));

        sb.AppendLine();
        var craftPrice = _building.GetCraftPrice(buildingsData, rarity);
        sb.Append(craftPrice.GetPriceView(session));
        var craftTimeInSeconds = _building.GetCraftTimeInSeconds(buildingsData, rarity);
        var dtNow = DateTime.UtcNow;
        var timeSpan = dtNow.AddSeconds(craftTimeInSeconds) - dtNow;
        sb.AppendLine(timeSpan.GetView(session, withCaption: true)
            + (session.player.IsPremiumActive() ? $" {Emojis.StatPremium}" : string.Empty));

        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_header_ours"));
        var playerResources = new ResourceData[craftPrice.Length];
        for (var i = 0; i < playerResources.Length; i++)
        {
            var playerAmount = session.player.resources.GetValue(craftPrice[i].resourceId);
            playerResources[i] = craftPrice[i] with { amount = playerAmount };
        }
        sb.Append(playerResources.GetLocalizedView(session));

        ClearButtons();
        RegisterButton(Emojis.ButtonCraft + Localization.Get(session, "dialog_craft_start_craft_button"),
            () => StartCraftItem(itemType, rarity));
        RegisterBackButton(() => StartSelectRarity(itemType));

        await SendDialogMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private async Task StartCraftItem(ItemType itemType, Rarity rarity)
    {
        var playerResources = session.player.resources;
        var requiredResources = _building.GetCraftPrice(buildingsData, rarity);
        var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
        if (successfullPurchase)
        {
            Program.logger.Info($"CRAFT | User {session.actualUser} start craft item {itemType} - {rarity}");
            _building.StartCraft(buildingsData, itemType, rarity);
            await new CraftInProgressDialog(session, _building).Start().FastAwait();
            return;
        }

        var notEnoughMaterials = notEnoughResources.Where(x => x.resourceId.IsCraftResource()).ToArray();
        if (notEnoughMaterials.Length > 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(session, "dialog_craft_not_enough_materials"));
            sb.AppendLine();
            sb.Append(notEnoughMaterials.GetLocalizedView(session));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_craft_how_to_get_materials"));

            ClearButtons();
            RegisterBackButton(() => ShowCraftPrice(itemType, rarity));

            await SendDialogMessage(sb, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
            onSuccess: async () => await new CraftNewItemDialog(session, _building).StartCraftItem(itemType, rarity).FastAwait(),
            onCancel: async () => await new CraftNewItemDialog(session, _building).ShowCraftPrice(itemType, rarity).FastAwait());
        await buyResourcesDialog.Start().FastAwait();
    }

}
