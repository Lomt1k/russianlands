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
        }

        private void RegisterCategoryButton(BuildingCategory category)
        {
            RegisterButton(category.GetLocalization(session), () => ShowBuildingsCategory(category));
        }

        public async Task ShowBuildingsCategory(BuildingCategory category)
        {
            ClearButtons();
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_back_to_categories")}",
                () => Start());

            var text = $"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>";
            await SendDialogMessage(text, GetOneLineKeyboard());
            await _inspectorPanel.ShowBuildingsList(category, asNewMessage: true);
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
                await new BuildingInfoDialog(session, inspectedBuilding).TryStartConstruction();
                return;
            }
            await new BuildingInfoDialog(session, inspectedBuilding).Start();
        }

        public async Task StartWithShowBuildingInfo(BuildingBase building)
        {
            await SendHeader();
            await new BuildingInfoDialog(session, building).Start();
        }

        private async Task SendHeader()
        {
            ClearButtons();
            RegisterCategoryButton(BuildingCategory.General);
            RegisterCategoryButton(BuildingCategory.Storages);
            RegisterCategoryButton(BuildingCategory.Production);
            RegisterCategoryButton(BuildingCategory.Training);

            RegisterBackButton(() => new TownDialog(session, TownEntryReason.BackFromInnerDialog).Start());

            var sb = new StringBuilder();
            sb.Append($"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>");
            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetKeyboardWithRowSizes(2, 2, 1));
        }

    }
}
