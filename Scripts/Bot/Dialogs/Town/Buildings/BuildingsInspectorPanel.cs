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
    public class BuildingsInspectorPanel : DialogPanelBase
    {
        private ProfileBuildingsData _buildingsData => session.profile.buildingsData;

        public BuildingsInspectorPanel(DialogBase _dialog, byte _panelId) : base(_dialog, _panelId)
        {
        }

        public override async Task SendAsync()
        {
            await ShowNotifications()
                .ConfigureAwait(false);
        }

        public async Task ShowNotifications()
        {
            ClearButtons();
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

            AppendGeneralResources(sb);
            AppendProductionInfo(sb);
            RegisterButton(Localization.Get(session, "dialog_buildings_get_resources"), () => new TryCollectResourcesDialog(session).Start());

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetMultilineKeyboard(), asNewMessage: true)
                .ConfigureAwait(false);
        }

        private void AppendGeneralResources(StringBuilder sb)
        {
            string resources = session.player.resources.GetGeneralResourcesView();
            sb.AppendLine(resources);
        }

        private void AppendProductionInfo(StringBuilder sb)
        {
            bool isLimitReached = false;
            var resourcesToShow = new Dictionary<ResourceType, int>();
            var productionBuildings = session.player.buildings.GetBuildingsByCategory(BuildingCategory.Production);
            foreach (ProductionBuildingBase building in productionBuildings)
            {
                if (!building.IsBuilt(_buildingsData))
                    continue;

                var farmedAmount = building.GetFarmedResourceAmount(_buildingsData);
                var limit = building.GetCurrentLevelResourceLimit(_buildingsData);

                AddToShow(building.resourceType, farmedAmount);
                if (farmedAmount >= limit)
                {
                    isLimitReached = true;
                }
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "resource_header_producted"));
            sb.AppendLine(ResourceHelper.GetCompactResourcesView(resourcesToShow));

            if (isLimitReached)
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

        public async Task ShowBuildingsList(BuildingCategory category, bool asNewMessage)
        {
            await RemoveKeyboardFromLastMessage().ConfigureAwait(false);
            var sb = new StringBuilder();
            sb.Append(category.GetLocalization(session).Bold());
            var buildings = session.player.buildings.GetBuildingsByCategory(category);
            var sortedBuildings = buildings.OrderByDescending(x => x.IsBuilt(_buildingsData)).ThenBy(x => x.buildingData.levels[0].requiredTownHall);

            foreach (var building in sortedBuildings)
            {
                var name = GetPrefix(building, _buildingsData) + building.GetLocalizedName(session, _buildingsData);
                RegisterButton(name, () => ShowBuildingInfo(building));
            }

            TryAppendTooltip(sb);
            await SendPanelMessage(sb, GetListKeyboard(category), asNewMessage)
                .ConfigureAwait(false);
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
            switch (category)
            {
                case BuildingCategory.Storages:
                    return GetKeyboardWithRowSizes(1, 2, 2);
                case BuildingCategory.Production:
                    return GetKeyboardWithFixedRowSize(2);
                default:
                    return GetMultilineKeyboard();
            }
        }

        private async Task ShowBuildingInfo(BuildingBase building)
        {
            await new BuildingInfoDialog(session, building).Start()
                .ConfigureAwait(false);
        }

        

    }
}
