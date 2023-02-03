using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Resources;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Shop;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
{
    public class BuildingInfoDialog : DialogBase
    {
        public static int maxConstructions = 2;
        public static int maxConstructionsPremium = 4;

        private ProfileBuildingsData _buildingsData => session.profile.buildingsData;
        private BuildingBase building;

        public BuildingInfoDialog(GameSession _session, BuildingBase building) : base(_session)
        {
            this.building = building;
        }

        public override async Task Start()
        {
            if (!building.IsBuilt(_buildingsData) && !building.IsUnderConstruction(_buildingsData))
            {
                await ShowConstructionAvailableInfo()
                    .ConfigureAwait(false);
                return;
            }
            await ShowBuildingCurrentLevelInfo()
                .ConfigureAwait(false);
        }

        public async Task ShowBuildingCurrentLevelInfo()
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

            bool isUnderConstruction = building.IsUnderConstruction(_buildingsData);
            if (!building.IsMaxLevel(_buildingsData))
            {
                var currentLevel = building.GetCurrentLevel(_buildingsData);
                var nextLevel = building.buildingData.levels[currentLevel];
                if (isUnderConstruction)
                {
                    var time = building.GetEndConstructionTime(_buildingsData);
                    var secondsToEnd = (int)(time - DateTime.UtcNow).TotalSeconds;
                    var diamondsForBoost = ResourceHelper.CalculateConstructionBoostPriceInDiamonds(secondsToEnd);
                    var priceView = ResourceType.Diamond.GetEmoji().ToString() + diamondsForBoost;
                    var buttonText = nextLevel.isBoostAvailable
                        ? Localization.Get(session, "menu_item_boost_free_button")
                        : Localization.Get(session, "menu_item_boost_button", priceView);
                    RegisterButton(buttonText, () => TryBoostConstructionForDiamonds());
                }
                else
                {
                    var playerTownHall = BuildingType.TownHall.GetBuilding().GetCurrentLevel(_buildingsData);
                    var nextLevelIcon = playerTownHall < nextLevel.requiredTownHall
                        ? Emojis.ElementLocked
                        : Emojis.ElementLevelUp;
                    RegisterButton(nextLevelIcon + Localization.Get(session, "dialog_buildings_construction_button"),
                        () => ShowConstructionAvailableInfo());
                }
            }
            var specialButtons = building.GetSpecialButtons(session, _buildingsData);
            foreach (var button in specialButtons)
            {
                RegisterButton(button.Key, () => button.Value());
            }
            var category = building.buildingType.GetCategory();
            RegisterBackButton(category.GetLocalization(session),
                () => new BuildingsDialog(session).ShowBuildingsCategory(category));
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_buildings") + Emojis.ButtonBuildings,
                () => new BuildingsDialog(session).Start());

            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private async Task TryBoostConstructionForDiamonds()
        {
            if (!building.IsUnderConstruction(_buildingsData) || building.IsConstructionCanBeFinished(_buildingsData))
            {
                var message = Emojis.ElementClock + Localization.Get(session, "dialog_buildings_construction_boost_expired");
                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowBuildingCurrentLevelInfo());
                await SendDialogMessage(message, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            var currentLevel = building.GetCurrentLevel(_buildingsData);
            var nextLevel = building.buildingData.levels[currentLevel];

            var time = building.GetEndConstructionTime(_buildingsData);
            var secondsToEnd = (int)(time - DateTime.UtcNow).TotalSeconds;
            var requiredDiamonds = nextLevel.isBoostAvailable ? 0 : ResourceHelper.CalculateConstructionBoostPriceInDiamonds(secondsToEnd);

            var playerResources = session.player.resources;
            var successsPurchase = playerResources.TryPurchase(ResourceType.Diamond, requiredDiamonds, out var notEnoughDiamonds);
            if (successsPurchase)
            {
                building.LevelUp(_buildingsData);

                var sb = new StringBuilder();
                sb.AppendLine(building.GetLocalizedName(session, _buildingsData).Bold());
                sb.AppendLine();
                sb.AppendLine(Emojis.ElementConstruction + Localization.Get(session, "dialog_buildings_construction_boosted"));
                if (requiredDiamonds > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceType.Diamond.GetLocalizedView(session, requiredDiamonds));
                }

                ClearButtons();
                RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowBuildingCurrentLevelInfo());

                await SendDialogMessage(sb, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            ClearButtons();
            var text = Localization.Get(session, "resource_not_enough_diamonds", Emojis.SmileSad);
            RegisterButton(Emojis.ButtonShop + Localization.Get(session, "menu_item_shop"),
                () => new ShopDialog(session).Start());
            RegisterBackButton(() => ShowBuildingCurrentLevelInfo());

            await SendDialogMessage(text, GetMultilineKeyboard()).ConfigureAwait(false);
        }

        private async Task ShowConstructionAvailableInfo()
        {
            if (building.IsMaxLevel(_buildingsData))
            {
                await ShowBuildingCurrentLevelInfo()
                    .ConfigureAwait(false);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine(building.GetNextLevelLocalizedName(session, _buildingsData).Bold());
            sb.AppendLine();
            sb.AppendLine(building.GetNextLevelInfo(session, _buildingsData));

            var level = building.GetCurrentLevel(_buildingsData);
            var levelData = building.buildingData.levels[level];

            sb.AppendLine();
            var requiredResources = GetRequiredResourcesForConstruction();
            sb.Append(ResourceHelper.GetPriceView(session, requiredResources));
            var dtNow = DateTime.UtcNow;
            var timeSpan = (dtNow.AddSeconds(levelData.constructionTime) - dtNow);
            sb.AppendLine(timeSpan.GetView(session, withCaption: true));

            var playerTownHall = BuildingType.TownHall.GetBuilding().GetCurrentLevel(_buildingsData);
            if (playerTownHall < levelData.requiredTownHall)
            {
                sb.AppendLine();
                var localization = Localization.Get(session, "dialog_buildings_required_town_hall", levelData.requiredTownHall);
                sb.AppendLine(Emojis.ElementWarningGrey.ToString() + localization);
            }
            else
            {
                AppendOurResources(sb, requiredResources);
                AppendSpecialConstructionWarnings(sb);
            }


            ClearButtons();
            if (playerTownHall >= levelData.requiredTownHall)
            {
                RegisterButton(Emojis.ElementConstruction + Localization.Get(session, "dialog_buildings_start_construction_button"),
                    () => TryStartConstruction());
            }

            if (building.IsBuilt(_buildingsData))
            {
                RegisterBackButton(() => ShowBuildingCurrentLevelInfo());
            }
            else
            {
                var category = building.buildingType.GetCategory();
                RegisterBackButton(category.GetLocalization(session), () => new BuildingsDialog(session).ShowBuildingsCategory(category));
            }
            RegisterDoubleBackButton(Localization.Get(session, "menu_item_buildings") + Emojis.ButtonBuildings,
                () => new BuildingsDialog(session).Start());

            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetMultilineKeyboardWithDoubleBack())
                .ConfigureAwait(false);
        }

        private void AppendOurResources(StringBuilder sb, Dictionary<ResourceType,int> requiredResources)
        {
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_ours"));
            var ourResources = new Dictionary<ResourceType, int>();
            foreach (var kvp in requiredResources)
            {
                if (kvp.Value > 0)
                {
                    var resourceType = kvp.Key;
                    ourResources.Add(resourceType, session.player.resources.GetValue(resourceType));
                }
            }
            sb.Append(ResourceHelper.GetResourcesView(session, ourResources));
        }

        private void AppendSpecialConstructionWarnings(StringBuilder sb)
        {
            if (building is TrainingBuildingBase trainingBuilding)
            {
                bool hasActiveTraining = trainingBuilding.GetFirstTrainingUnitIndex(_buildingsData) != -1
                    || trainingBuilding.GetSecondTrainingUnitIndex(_buildingsData) != -1;
                if (hasActiveTraining)
                {
                    sb.AppendLine();
                    sb.AppendLine(Emojis.ElementWarningRed.ToString() + Localization.Get(session, "building_training_break_warning"));
                }
            }
        }

        public async Task TryStartConstruction()
        {
            if (building.IsMaxLevel(_buildingsData))
            {
                await ShowBuildingCurrentLevelInfo()
                    .ConfigureAwait(false);
                return;
            }
            if (building.IsStartConstructionBlocked(_buildingsData, out var blockReasonMessage))
            {
                ClearButtons();
                RegisterBackButton(() => ShowConstructionAvailableInfo());
                await SendDialogMessage(blockReasonMessage, GetOneLineKeyboard())
                    .ConfigureAwait(false);
                return;
            }

            var isPremiumActive = session.profile.data.IsPremiumActive();
            var constructionsLimit = isPremiumActive ? maxConstructionsPremium : maxConstructions;
            var allBuildings = session.player.buildings.GetAllBuildings();
            var constructions = allBuildings.Where(x => x.IsUnderConstruction(_buildingsData) && !x.IsConstructionCanBeFinished(_buildingsData)).ToArray();
            if (constructions.Length >= constructionsLimit)
            {
                await ShowConstructionsLimitMessage(constructions)
                    .ConfigureAwait(false);
                return;
            }

            var requiredResources = GetRequiredResourcesForConstruction();
            var playerResources = session.player.resources;
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                building.StartConstruction(_buildingsData);
                await ShowBuildingCurrentLevelInfo()
                    .ConfigureAwait(false);
                return;
            }

            if (IsStorageUpgradeRequired(requiredResources, out var storageBuilding))
            {
                await ShowStorageUpgradeRequired(storageBuilding)
                    .ConfigureAwait(false);
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new BuildingsDialog(session).StartFromBuyResourcesDialog(building, true).ConfigureAwait(false),
                onCancel: async () => await new BuildingsDialog(session).StartFromBuyResourcesDialog(building, false).ConfigureAwait(false));
            await buyResourcesDialog.Start()
                .ConfigureAwait(false);
        }

        private async Task ShowConstructionsLimitMessage(BuildingBase[] constructions)
        {
            ClearButtons();
            var sb = new StringBuilder();
            var data = session.profile.buildingsData;
            sb.AppendLine(Localization.Get(session, "dialog_buildings_construction_limit"));
            sb.AppendLine();

            for (int i = 0; i < constructions.Length; i++)
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
            RegisterBackButton(() => ShowBuildingCurrentLevelInfo());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

        private Dictionary<ResourceType, int> GetRequiredResourcesForConstruction()
        {
            if (building.IsMaxLevel(_buildingsData))
                return new Dictionary<ResourceType, int>();

            var level = building.GetCurrentLevel(_buildingsData);
            var levelData = building.buildingData.levels[level];
            return new Dictionary<ResourceType, int>
            {
                {ResourceType.Gold, levelData.requiredGold },
                {ResourceType.Herbs, levelData.requiredHerbs },
                {ResourceType.Wood, levelData.requiredWood },
            };
        }

        private bool IsStorageUpgradeRequired(Dictionary<ResourceType, int> requiredResources, out StorageBuildingBase storageBuilding)
        {
            storageBuilding = null;
            var playerResources = session.player.resources;

            foreach (var requiredResource in requiredResources)
            {
                var resourceType = requiredResource.Key;
                var resourceAmount = requiredResource.Value;
                if (resourceAmount > playerResources.GetResourceLimit(resourceType))
                {
                    switch (resourceType)
                    {
                        case ResourceType.Gold:
                            storageBuilding = (StorageBuildingBase)BuildingType.GoldStorage.GetBuilding();
                            return true;
                        case ResourceType.Food:
                            storageBuilding = (StorageBuildingBase)BuildingType.FoodStorage.GetBuilding();
                            return true;
                        case ResourceType.Herbs:
                            storageBuilding = (StorageBuildingBase)BuildingType.HerbsStorage.GetBuilding();
                            return true;
                        case ResourceType.Wood:
                            storageBuilding = (StorageBuildingBase)BuildingType.WoodStorage.GetBuilding();
                            return true;
                    }
                }
            }
            return false;
        }

        private async Task ShowStorageUpgradeRequired(StorageBuildingBase storageToUpgrade)
        {
            ClearButtons();
            var sb = new StringBuilder();

            sb.AppendLine(Localization.Get(session, "dialog_buildings_storage_upgrade_required"));
            sb.AppendLine(Emojis.ElementSmallBlack + storageToUpgrade.GetLocalizedName(session, _buildingsData));

            RegisterButton(Localization.Get(session, "dialog_buildings_go_to_storage_button"), () => new BuildingInfoDialog(session, storageToUpgrade).Start());
            RegisterBackButton(() => ShowConstructionAvailableInfo());

            await SendDialogMessage(sb, GetMultilineKeyboard())
                .ConfigureAwait(false);
        }

    }
}
