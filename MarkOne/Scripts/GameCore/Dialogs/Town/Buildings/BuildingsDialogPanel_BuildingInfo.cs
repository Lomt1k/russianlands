using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs;
using MarkOne.Scripts.GameCore.Dialogs.Resources;
using MarkOne.Scripts.GameCore.Dialogs.Town.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Buildings;

public partial class BuildingsDialogPanel : DialogPanelBase
{
    public static int maxConstructions = 2;
    public static int maxConstructionsPremium = 4;

    public async Task ShowBuilding(BuildingBase building)
    {
        if (!building.IsBuilt(_buildingsData) && !building.IsUnderConstruction(_buildingsData))
        {
            await ShowConstructionAvailableInfo(building).FastAwait();
            return;
        }
        await ShowBuildingCurrentLevelInfo(building).FastAwait();
    }

    private async Task ShowBuildingCurrentLevelInfo(BuildingBase building)
    {
        ClearButtons();
        var sb = new StringBuilder();
        sb.AppendLine(building.GetLocalizedName(session, _buildingsData).Bold());
        sb.AppendLine();
        if (building.GetCurrentLevel(_buildingsData) > 0)
        {
            sb.AppendLine(building.GetCurrentLevelInfo(session, _buildingsData));
        }
        else
        {
            sb.AppendLine(building.GetNextLevelInfo(session, _buildingsData));
        }

        var updates = building.GetUpdates(session, _buildingsData, onlyImportant: false);
        if (updates.Count > 0)
        {
            sb.AppendLine();
            foreach (var update in updates)
            {
                sb.AppendLine(Emojis.ElementSmallBlack + update);
            }
        }

        var isUnderConstruction = building.IsUnderConstruction(_buildingsData);
        if (!building.IsMaxLevel(_buildingsData))
        {
            var currentLevel = building.GetCurrentLevel(_buildingsData);
            var nextLevel = building.buildingData.levels[currentLevel];
            if (isUnderConstruction)
            {
                var time = building.GetEndConstructionTime(_buildingsData);
                var secondsToEnd = (int)(time - DateTime.UtcNow).TotalSeconds;
                var diamondsForBoost = ResourceHelper.CalculateConstructionBoostPriceInDiamonds(secondsToEnd);
                var priceView = diamondsForBoost.GetCompactView(shortView: false);
                var buttonText = nextLevel.isBoostAvailable
                    ? Localization.Get(session, "menu_item_boost_free_button")
                    : Localization.Get(session, "menu_item_boost_button", priceView);
                RegisterButton(buttonText, () => TryBoostConstructionForDiamonds(building));
            }
            else
            {
                var playerTownHall = BuildingId.TownHall.GetBuilding().GetCurrentLevel(_buildingsData);
                var nextLevelIcon = playerTownHall < nextLevel.requiredTownHall
                    ? Emojis.ElementLocked
                    : Emojis.ElementLevelUp;
                RegisterButton(nextLevelIcon + Localization.Get(session, "dialog_buildings_construction_button"),
                    () => ShowConstructionAvailableInfo(building));
            }
        }
        var specialButtons = building.GetSpecialButtons(session, _buildingsData);
        foreach (var button in specialButtons)
        {
            RegisterButton(button.Key, () => button.Value());
        }
        var category = building.buildingId.GetCategory();
        RegisterBackButton(category.GetLocalization(session), () => ShowBuildingsList(category));
        RegisterDoubleBackButton(Localization.Get(session, "menu_item_buildings") + Emojis.ButtonBuildings, () => ShowCategories());

        TryAppendTooltip(sb);
        await SendPanelMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private async Task TryBoostConstructionForDiamonds(BuildingBase building)
    {
        ClearButtons();
        if (!building.IsUnderConstruction(_buildingsData) || building.IsConstructionCanBeFinished(_buildingsData))
        {
            var message = Emojis.ElementClock + Localization.Get(session, "dialog_buildings_construction_boost_expired");
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowBuildingCurrentLevelInfo(building));
            await SendPanelMessage(message, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var currentLevel = building.GetCurrentLevel(_buildingsData);
        var nextLevel = building.buildingData.levels[currentLevel];

        var time = building.GetEndConstructionTime(_buildingsData);
        var secondsToEnd = (int)(time - DateTime.UtcNow).TotalSeconds;
        var requiredDiamonds = nextLevel.isBoostAvailable ? new ResourceData()
            : ResourceHelper.CalculateConstructionBoostPriceInDiamonds(secondsToEnd);

        var playerResources = session.player.resources;
        var successsPurchase = playerResources.TryPurchase(requiredDiamonds, out var notEnoughDiamonds);
        if (successsPurchase)
        {
            building.LevelUp(_buildingsData);

            var sb = new StringBuilder();
            sb.AppendLine(building.GetLocalizedName(session, _buildingsData).Bold());
            sb.AppendLine();
            sb.AppendLine(Emojis.ElementConstruction + Localization.Get(session, "dialog_buildings_construction_boosted"));
            if (requiredDiamonds.amount > 0)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                sb.AppendLine(requiredDiamonds.GetLocalizedView(session));
            }

            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowBuildingCurrentLevelInfo(building));

            await SendPanelMessage(sb, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
        RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
        RegisterBackButton(() => ShowBuildingCurrentLevelInfo(building));

        await SendPanelMessage(text, GetMultilineKeyboard()).FastAwait();
    }

    private async Task ShowConstructionAvailableInfo(BuildingBase building)
    {
        var sb = new StringBuilder();
        sb.AppendLine(building.GetNextLevelLocalizedName(session, _buildingsData).Bold());
        sb.AppendLine();
        sb.AppendLine(building.GetNextLevelInfo(session, _buildingsData));

        var level = building.GetCurrentLevel(_buildingsData);
        var levelData = building.buildingData.levels[level];

        sb.AppendLine();
        var requiredResources = GetRequiredResourcesForConstruction(building);
        sb.Append(requiredResources.GetPriceView(session));
        var dtNow = DateTime.UtcNow;
        var constuctionTime = session.player.IsPremiumActive()
            ? levelData.constructionTime * 3 / 4
            : levelData.constructionTime;
        var timeSpan = TimeSpan.FromSeconds(constuctionTime);
        sb.AppendLine(timeSpan.GetView(session, withCaption: true)
            + (session.player.IsPremiumActive() ? $" {Emojis.StatPremium}" : string.Empty));

        var playerTownHall = BuildingId.TownHall.GetBuilding().GetCurrentLevel(_buildingsData);
        if (playerTownHall < levelData.requiredTownHall)
        {
            sb.AppendLine();
            var localization = Localization.Get(session, "dialog_buildings_required_town_hall", levelData.requiredTownHall);
            sb.AppendLine(Emojis.ElementWarningGrey.ToString() + localization);
        }
        else
        {
            AppendOurResources(sb, requiredResources);
            AppendSpecialConstructionWarnings(sb, building);
        }


        ClearButtons();
        if (playerTownHall >= levelData.requiredTownHall)
        {
            RegisterButton(Emojis.ElementConstruction + Localization.Get(session, "dialog_buildings_start_construction_button"),
                () => TryStartConstruction(building));
        }

        if (building.IsBuilt(_buildingsData))
        {
            RegisterBackButton(() => ShowBuildingCurrentLevelInfo(building));
        }
        else
        {
            var category = building.buildingId.GetCategory();
            RegisterBackButton(category.GetLocalization(session), () => ShowBuildingsList(category));
        }
        RegisterDoubleBackButton(Localization.Get(session, "menu_item_buildings") + Emojis.ButtonBuildings, ShowCategories);

        TryAppendTooltip(sb);
        await SendPanelMessage(sb, GetMultilineKeyboardWithDoubleBack()).FastAwait();
    }

    private void AppendOurResources(StringBuilder sb, IEnumerable<ResourceData> requiredResources)
    {
        sb.AppendLine();
        sb.AppendLine(Localization.Get(session, "resource_header_ours"));
        var ourResources = new List<ResourceData>();
        foreach (var resourceData in requiredResources)
        {
            if (resourceData.amount > 0)
            {
                var ourAmount = session.player.resources.GetValue(resourceData.resourceId);
                ourResources.Add(resourceData with { amount = ourAmount });
            }
        }
        sb.Append(ourResources.GetLocalizedView(session));
    }

    private void AppendSpecialConstructionWarnings(StringBuilder sb, BuildingBase building)
    {
        if (!building.IsBuilt(_buildingsData))
            return;

        var warningText = building.GetSpecialConstructionWarning(_buildingsData, session);
        if (!string.IsNullOrEmpty(warningText))
        {
            sb.AppendLine();
            sb.AppendLine(Emojis.ElementWarningRed.ToString() + warningText);
        }
    }

    public async Task TryStartConstruction(BuildingBase building)
    {
        if (building.IsMaxLevel(_buildingsData))
        {
            await ShowBuildingCurrentLevelInfo(building).FastAwait();
            return;
        }
        if (building.IsStartConstructionBlocked(_buildingsData, out var blockReasonMessage))
        {
            ClearButtons();
            RegisterBackButton(() => ShowConstructionAvailableInfo(building));
            await SendPanelMessage(blockReasonMessage, GetOneLineKeyboard()).FastAwait();
            return;
        }

        var isPremiumActive = session.profile.data.IsPremiumActive();
        var constructionsLimit = isPremiumActive ? maxConstructionsPremium : maxConstructions;
        var allBuildings = session.player.buildings.GetAllBuildings();
        var constructions = allBuildings.Where(x => x.IsUnderConstruction(_buildingsData) && !x.IsConstructionCanBeFinished(_buildingsData)).ToArray();
        if (constructions.Length >= constructionsLimit)
        {
            await ShowConstructionsLimitMessage(constructions, building).FastAwait();
            return;
        }

        var requiredResources = GetRequiredResourcesForConstruction(building);
        var playerResources = session.player.resources;
        var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
        if (successfullPurchase)
        {
            building.StartConstruction(_buildingsData);
            await ShowBuildingCurrentLevelInfo(building).FastAwait();
            return;
        }

        if (IsStorageUpgradeRequired(requiredResources, out var storageBuilding))
        {
            await ShowStorageUpgradeRequired(building, storageBuilding).FastAwait();
            return;
        }

        var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
            onSuccess: async () => await new BuildingsDialog(session).StartWithTryStartConstruction(building).FastAwait(),
            onCancel: async () => await new BuildingsDialog(session).StartWithShowBuilding(building).FastAwait());
        await buyResourcesDialog.Start().FastAwait();
    }

    private async Task ShowConstructionsLimitMessage(BuildingBase[] constructions, BuildingBase building)
    {
        ClearButtons();
        var sb = new StringBuilder();
        var data = session.profile.buildingsData;
        sb.AppendLine(Localization.Get(session, "dialog_buildings_construction_limit"));
        sb.AppendLine();

        for (var i = 0; i < constructions.Length; i++)
        {
            var charIcon = i % 2 == 0 ? Avatar.BuilderA : Avatar.BuilderB;
            sb.Append(charIcon.GetEmoji() + Localization.Get(session, $"dialog_buildings_worker_{i}_name").Bold());

            var buildingName = constructions[i].GetLocalizedName(session, data);
            var time = (constructions[i].GetEndConstructionTime(data) - DateTime.UtcNow).GetShortView(session);
            sb.AppendLine($"{buildingName} {time}");
        }

        if (!session.profile.data.IsPremiumActive())
        {
            sb.AppendLine();
            sb.AppendLine(Emojis.StatPremium + Localization.Get(session, "menu_item_premium").Bold());
            sb.AppendLine(Localization.Get(session, "dialog_buildings_construction_limit_can_buy_premium", maxConstructionsPremium));
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"), () => new ShopDialog(session).Start());
        }
        RegisterBackButton(() => ShowBuildingCurrentLevelInfo(building));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

    private ResourceData[] GetRequiredResourcesForConstruction(BuildingBase building)
    {
        if (building.IsMaxLevel(_buildingsData))
            return new ResourceData[0];

        var level = building.GetCurrentLevel(_buildingsData);
        var levelData = building.buildingData.levels[level];
        return new ResourceData[]
        {
            new ResourceData(ResourceId.Gold, levelData.requiredGold),
            new ResourceData(ResourceId.Herbs, levelData.requiredHerbs),
            new ResourceData(ResourceId.Wood, levelData.requiredWood),
        };
    }

    private bool IsStorageUpgradeRequired(IEnumerable<ResourceData> requiredResources, out StorageBuildingBase storageBuilding)
    {
        storageBuilding = null;
        var playerResources = session.player.resources;

        foreach (var (resourceId, amount) in requiredResources)
        {
            if (amount > playerResources.GetResourceLimit(resourceId))
            {
                switch (resourceId)
                {
                    case ResourceId.Gold:
                        storageBuilding = (StorageBuildingBase)BuildingId.GoldStorage.GetBuilding();
                        return true;
                    case ResourceId.Food:
                        storageBuilding = (StorageBuildingBase)BuildingId.FoodStorage.GetBuilding();
                        return true;
                    case ResourceId.Herbs:
                        storageBuilding = (StorageBuildingBase)BuildingId.HerbsStorage.GetBuilding();
                        return true;
                    case ResourceId.Wood:
                        storageBuilding = (StorageBuildingBase)BuildingId.WoodStorage.GetBuilding();
                        return true;
                }
            }
        }
        return false;
    }

    private async Task ShowStorageUpgradeRequired(BuildingBase building, StorageBuildingBase storageToUpgrade)
    {
        ClearButtons();
        var sb = new StringBuilder();

        sb.AppendLine(Localization.Get(session, "dialog_buildings_storage_upgrade_required"));
        sb.AppendLine(Emojis.ElementSmallBlack + storageToUpgrade.GetLocalizedName(session, _buildingsData));

        RegisterButton(Localization.Get(session, "dialog_buildings_go_to_storage_button"), () => ShowBuilding(storageToUpgrade));
        RegisterBackButton(() => ShowConstructionAvailableInfo(building));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }
}
