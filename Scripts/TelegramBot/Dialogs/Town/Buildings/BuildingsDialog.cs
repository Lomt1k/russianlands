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

            RegisterButton($"{Emojis.elements[Element.Back]} " + Localization.Get(session, "menu_item_back_button"),
                () => new TownEntryDialog(session, TownEntryReason.BackFromInnerDialog).Start());
        }

        private void RegisterCategoryButton(BuildingCategory category)
        {
            RegisterButton(category.GetLocalization(session), () => _inspectorPanel.ShowBuildingsList(category, asNewMessage: true));
        }

        public override async Task Start()
        {
            var header = $"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(2, 2, 1));
            await SendPanelsAsync();
        }

        public async Task StartFromBuyResourcesDialog(BuildingBase inspectedBuilding, bool successfullPurchase)
        {
            var header = $"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>";
            await messageSender.SendTextDialog(session.chatId, header, GetKeyboardWithRowSizes(2, 2, 1));
            if (successfullPurchase)
            {
                await _inspectorPanel.TryStartConstruction(inspectedBuilding);
                return;
            }
            await _inspectorPanel.ShowBuildingCurrentLevelInfo(inspectedBuilding);
        }

    }
}
