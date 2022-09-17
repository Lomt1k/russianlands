using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Resources;
using System.Collections.Generic;
using System;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Resources;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Shop;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings
{
    public class BuildingsInspectorPanel : DialogPanelBase
    {
        private ProfileBuildingsData _buildingsData => session.profile.buildingsData;

        public BuildingsInspectorPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowNotifications();
        }

        public async Task ShowNotifications()
        {
            ClearButtons();
            var sb = new StringBuilder();
            var playerBuildings = session.player.buildings;
            if (playerBuildings.HasImportantUpdates())
            {
                var allBuildings = playerBuildings.GetAllBuildings();
                foreach (var building in allBuildings)
                {
                    var updates = building.GetUpdates(session, session.profile.buildingsData);
                    if (updates.Count < 1)
                        continue;

                    var header = $"<pre>{building.GetLocalizedName(session, session.profile.buildingsData)}:</pre>";
                    sb.AppendLine(header);

                    foreach (var update in updates)
                    {
                        sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
                    }
                    sb.AppendLine();
                }
            }

            // TODO: Инфа о добытых ресурсах
            sb.AppendLine("<pre>Добытые ресурсы:</pre>");
            sb.AppendLine("[Инфа о добытых ресурсах]");

            RegisterButton(Localization.Get(session, "dialog_buildings_get_resources"), null);

            lastMessage = lastMessage == null 
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString(), GetMultilineKeyboard());
        }

        public async Task ShowBuildingsList(BuildingCategory category, bool asNewMessage)
        {
            await RemoveKeyboardFromLastMessage();
            var text = $"<b>{category.GetLocalization(session)}</b>";
            var buildings = session.player.buildings.GetBuildingsByCategory(category);
            var sortedBuildings = buildings.OrderBy(x => x.IsBuilt(_buildingsData)).ThenBy(x => x.buildingData.levels[0].requiredTownHall);

            foreach (var building in sortedBuildings)
            {
                var name = GetPrefix(building, _buildingsData) + building.GetLocalizedName(session, _buildingsData);
                RegisterButton(name, () => ShowBuildingInfo(building));
            }
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_buildings")}",
                () => ShowNotifications());

            lastMessage = asNewMessage || lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private string GetPrefix(BuildingBase building, ProfileBuildingsData data)
        {
            if (building.IsUnderConstruction(data))
                return Emojis.elements[Element.Construction] + Emojis.space;

            if (building.IsBuilt(data))
                return string.Empty;

            return building.IsNextLevelUnlocked(data)
                ? Emojis.elements[Element.Plus] + Emojis.space
                : Emojis.elements[Element.Locked] + Emojis.space;
        }

        private async Task ShowBuildingInfo(BuildingBase building)
        {
            if (!building.IsBuilt(_buildingsData))
            {
                await ShowConstructionAvailableInfo(building);
                return;
            }
            await ShowBuildingCurrentLevelInfo(building);
        }

        public async Task ShowBuildingCurrentLevelInfo(BuildingBase building)
        {
            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{building.GetLocalizedName(session, _buildingsData)}</b>");
            sb.AppendLine();
            sb.AppendLine(building.GetCurrentLevelInfo(session, _buildingsData));

            var updates = building.GetUpdates(session, _buildingsData);
            if (updates.Count > 0)
            {
                sb.AppendLine();
                foreach (var update in updates)
                {
                    sb.AppendLine($"{Emojis.elements[Element.SmallWhite]} {update}");
                }
            }

            bool isUnderConstruction = building.IsUnderConstruction(_buildingsData);
            if (!building.IsMaxLevel(_buildingsData))
            {
                if (isUnderConstruction)
                {
                    var currentLevel = building.GetCurrentLevel(_buildingsData);
                    var nextLevel = building.buildingData.levels[currentLevel];

                    var time = building.GetEndConstructionTime(_buildingsData);
                    var secondsToEnd = (int)(time - DateTime.UtcNow).TotalSeconds;
                    var diamondsForBoost = ResourceHelper.CalculateConstructionBoostPriceInDiamonds(secondsToEnd);
                    var priceView = Emojis.resources[ResourceType.Diamond] + diamondsForBoost;
                    var buttonText = nextLevel.isBoostAvailable
                        ? Localization.Get(session, "menu_item_boost_free_button")
                        : string.Format(Localization.Get(session, "menu_item_boost_button"), priceView);
                    RegisterButton(buttonText, () => TryBoostConstructionForDiamonds(building));
                }
                else
                {
                    RegisterButton($"{Emojis.elements[Element.LevelUp]} {Localization.Get(session, "dialog_buildings_construction_button")}",
                        () => ShowConstructionAvailableInfo(building));
                }
            }
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_to_list_button")}",
                    () => ShowBuildingsList(building.buildingType.GetCategory(), asNewMessage: false));

            lastMessage = lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString(), GetMultilineKeyboard());
        }

        private async Task TryBoostConstructionForDiamonds(BuildingBase building)
        {
            await RemoveKeyboardFromLastMessage();
            lastMessage = null;

            if (!building.IsUnderConstruction(_buildingsData) || building.IsConstructionCanBeFinished(_buildingsData))
            {
                var message = $"{Emojis.elements[Element.Construction]} {Localization.Get(session, "dialog_buildings_construction_boost_expired")}";
                await messageSender.SendTextMessage(session.chatId, message);
                await ShowBuildingCurrentLevelInfo(building);
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
                sb.AppendLine($"{Emojis.elements[Element.Construction]} {Localization.Get(session, "dialog_buildings_construction_boosted")}");
                if (requiredDiamonds > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Get(session, "resource_header_spent"));
                    sb.AppendLine(ResourceType.Diamond.GetLocalizedView(session, requiredDiamonds));
                }

                await messageSender.SendTextMessage(session.chatId, sb.ToString());
                await ShowBuildingCurrentLevelInfo(building);
                return;
            }

            ClearButtons();
            var text = string.Format(Localization.Get(session, "resource_not_enough_diamonds"), Emojis.smiles[Smile.Sad]);
            RegisterButton($"{Emojis.menuItems[MenuItem.Shop]} {Localization.Get(session, "menu_item_shop")}",
                () => new ShopDialog(session).Start());
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_button")}", () => ShowBuildingCurrentLevelInfo(building));

            lastMessage = lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, text, GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, text, GetMultilineKeyboard());
        }

        private async Task ShowConstructionAvailableInfo(BuildingBase building)
        {
            if (building.IsMaxLevel(_buildingsData))
            {
                await ShowBuildingCurrentLevelInfo(building);
                return;
            }

            ClearButtons();
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{building.GetNextLevelLocalizedName(session, _buildingsData)}</b>");
            sb.AppendLine();
            sb.AppendLine(building.GetNextLevelInfo(session, _buildingsData));

            var level = building.GetCurrentLevel(_buildingsData);
            var levelData = building.buildingData.levels[level];

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_buildings_construction_time"));
            var dtNow = DateTime.UtcNow;
            var timeSpan = (dtNow.AddSeconds(levelData.constructionTime) - dtNow);
            sb.AppendLine(timeSpan.GetView(session));

            sb.AppendLine();
            var requiredResources = GetRequiredResourcesForConstruction(building);
            sb.AppendLine(ResourceHelper.GetPriceView(session, requiredResources));

            var playerTownHall = BuildingType.TownHall.GetBuilding().GetCurrentLevel(_buildingsData);
            if (playerTownHall < levelData.requiredTownHall)
            {
                var localization = string.Format(Localization.Get(session, "dialog_buildings_required_town_hall"), levelData.requiredTownHall);
                sb.AppendLine($"{Emojis.elements[Element.WarningGrey]} {localization}");
            }


            // buttons
            if (playerTownHall >= levelData.requiredTownHall)
            {
                RegisterButton($"{Emojis.elements[Element.Construction]} {Localization.Get(session, "dialog_buildings_start_construction_button")}",
                    () => TryStartConstruction(building));
            }

            if (building.IsBuilt(_buildingsData))
            {
                RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "dialog_buildings_info_button")}",
                    () => ShowBuildingInfo(building));
            }
            else
            {
                RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_to_list_button")}",
                    () => ShowBuildingsList(building.buildingType.GetCategory(), asNewMessage: false));
            }            

            lastMessage = lastMessage == null
                ? await messageSender.SendTextMessage(session.chatId, sb.ToString(), GetMultilineKeyboard())
                : await messageSender.EditTextMessage(session.chatId, lastMessage.MessageId, sb.ToString(), GetMultilineKeyboard());
        }

        public async Task TryStartConstruction(BuildingBase building)
        {
            if (building.IsMaxLevel(_buildingsData))
            {
                await ShowBuildingCurrentLevelInfo(building);
                return;
            }

            var requiredResources = GetRequiredResourcesForConstruction(building);
            var playerResources = session.player.resources;
            var successfullPurchase = playerResources.TryPurchase(requiredResources, out var notEnoughResources);
            if (successfullPurchase)
            {
                building.StartConstruction(_buildingsData);
                await ShowBuildingCurrentLevelInfo(building);
                return;
            }

            var buyResourcesDialog = new BuyResourcesForDiamondsDialog(session, notEnoughResources,
                onSuccess: async () => await new BuildingsDialog(session).StartFromBuyResourcesDialog(building, true),
                onCancel: async () => await new BuildingsDialog(session).StartFromBuyResourcesDialog(building, false));
            await buyResourcesDialog.Start();
        }

        private Dictionary<ResourceType, int> GetRequiredResourcesForConstruction(BuildingBase building)
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

    }
}
