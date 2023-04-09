using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Resources;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
{
    public partial class BuildingsDialogPanel : DialogPanelBase
    {
        private ProfileBuildingsData _buildingsData => session.profile.buildingsData;

        public BuildingsDialogPanel(DialogWithPanel _dialog) : base(_dialog)
        {
        }

        public override async Task Start()
        {
            await ShowCategories().FastAwait();
        }

        public async Task ShowCategories()
        {
            var sb = new StringBuilder();
            var playerBuildings = session.player.buildings.GetAllBuildings();
            foreach (var building in playerBuildings)
            {
                var updates = building.GetUpdates(session, session.profile.buildingsData, onlyImportant: true);
                if (updates.Count < 1)
                    continue;

                var header = $"<pre>{building.GetLocalizedName(session, session.profile.buildingsData)}:</pre>";
                sb.AppendLine(header);

                foreach (var update in updates)
                {
                    sb.AppendLine(Emojis.ElementSmallBlack + update);
                }
                sb.AppendLine();
            }

            AppendProductionInfo(sb);

            ClearButtons();
            RegisterButton(Localization.Get(session, "dialog_buildings_get_resources"), () => TryCollectResources());
            RegisterCategoryButton(BuildingCategory.General);
            RegisterCategoryButton(BuildingCategory.Storages);
            RegisterCategoryButton(BuildingCategory.Production);
            RegisterCategoryButton(BuildingCategory.Training);

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetKeyboardWithRowSizes(1, 2, 2)).FastAwait();
        }

        private void RegisterCategoryButton(BuildingCategory category)
        {
            RegisterButton(category.GetLocalization(session), () => ShowBuildingsList(category));
        }

        private void AppendProductionInfo(StringBuilder sb)
        {
            bool neeedToShowLimitWarning = false;
            var resourcesToShow = new Dictionary<ResourceType, int>();
            var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
            foreach (ProductionBuildingBase building in productionBuildings)
            {
                if (!building.IsBuilt(_buildingsData))
                    continue;

                var farmedAmount = building.GetFarmedResourceAmount(_buildingsData);
                AddToShow(building.resourceType, farmedAmount);

                var limit = building.GetCurrentLevelResourceLimit(_buildingsData);
                var isFarmedLimitReached = farmedAmount >= limit;
                
                if (isFarmedLimitReached)
                {
                    var isStorageLimitReached = session.player.resources.IsResourceLimitReached(building.resourceType);
                    if (!isStorageLimitReached)
                    {
                        neeedToShowLimitWarning = true;
                    }
                }
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_producted"));
            sb.AppendLine(ResourceHelper.GetCompactResourcesView(resourcesToShow));

            if (neeedToShowLimitWarning)
            {
                sb.AppendLine();
                sb.AppendLine(Emojis.ElementWarningRed.ToString() + Localization.Get(session, "building_production_is_full"));
            }

            void AddToShow(ResourceType resourceType, int value)
            {
                if (resourcesToShow.ContainsKey(resourceType))
                    resourcesToShow[resourceType] += value;
                else
                    resourcesToShow.Add(resourceType, value);
            }
        }

        public async Task ShowBuildingsList(BuildingCategory category)
        {
            ClearButtons();

            var sb = new StringBuilder();
            sb.Append(category.GetLocalization(session).Bold());
            var buildings = session.player.buildings.GetBuildingsByCategory(category);
            var sortedBuildings = buildings.OrderByDescending(x => x.IsBuilt(_buildingsData)).ThenBy(x => x.buildingData.levels[0].requiredTownHall);
            foreach (var building in sortedBuildings)
            {
                var name = GetPrefix(building, _buildingsData) + building.GetLocalizedName(session, _buildingsData);
                RegisterButton(name, () => ShowBuilding(building));
            }
            RegisterBackButton(() => ShowCategories());

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetListKeyboard(category)).FastAwait();
        }

        private Emoji GetPrefix(BuildingBase building, ProfileBuildingsData data)
        {
            if (building.IsUnderConstruction(data))
                return Emojis.ElementConstruction;

            if (building.IsBuilt(data))
                return Emojis.Empty;

            return building.IsNextLevelUnlocked(data)
                ? Emojis.ElementPlus
                : Emojis.ElementLocked;
        }

        private InlineKeyboardMarkup GetListKeyboard(BuildingCategory category)
        {
            return category switch
            {
                BuildingCategory.Storages => GetKeyboardWithRowSizes(1, 2, 2, 1),
                BuildingCategory.Production => GetKeyboardWithFixedRowSize(2),
                _ => GetMultilineKeyboard()
            };
        }

        private async Task TryCollectResources()
        {
            ClearButtons();
            RegisterButton(Localization.Get(session, "menu_item_continue_button"), () => ShowCategories());
            var message = TryCollectResourcesDialog.StartLogicAndGetResultMessage(session);
            await SendPanelMessage(message, GetOneLineKeyboard()).FastAwait();
        }

        

    }
}
