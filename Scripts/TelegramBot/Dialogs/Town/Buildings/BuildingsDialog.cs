﻿using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Town.Buildings
{
    public class BuildingsDialog : DialogBase
    {
        private BuildingsInspectorPanel _inspectorPanel;

        public BuildingsDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new BuildingsInspectorPanel(this, 0);
            RegisterPanel(_inspectorPanel);

            RegisterCategoryButton(BuildingCategory.General);
            RegisterCategoryButton(BuildingCategory.Storages);
            RegisterCategoryButton(BuildingCategory.Production);
            RegisterCategoryButton(BuildingCategory.Training);

            RegisterBackButton(() => new TownDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        private void RegisterCategoryButton(BuildingCategory category)
        {
            RegisterButton(category.GetLocalization(session), () => _inspectorPanel.ShowBuildingsList(category, asNewMessage: true));
        }

        public override async Task Start()
        {
            if (session.tooltipController.HasTooltipToAppend(this))
            {
                await SendHeader();
                return;
            }
            await SendHeader();
            await SendPanelsAsync();
        }

        public async Task StartFromBuyResourcesDialog(BuildingBase inspectedBuilding, bool successfullPurchase)
        {
            await SendHeader();
            if (successfullPurchase)
            {
                await _inspectorPanel.TryStartConstruction(inspectedBuilding);
                return;
            }
            await _inspectorPanel.ShowBuildingCurrentLevelInfo(inspectedBuilding);
        }

        public async Task StartWithShowBuildingInfo(BuildingBase building)
        {
            await SendHeader();
            await _inspectorPanel.ShowBuildingCurrentLevelInfo(building);
        }

        private async Task SendHeader()
        {
            var sb = new StringBuilder();
            sb.Append($"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>");
            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetKeyboardWithRowSizes(2, 2, 1));
        }

    }
}
