using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
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
            RegisterButton($"{Emojis.elements[Element.Back]} {Localization.Get(session, "menu_item_buildings")} {Emojis.menuItems[MenuItem.Buildings]}",
                () => Start());
            RegisterTownButton(isFullBack: true);

            var text = $"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>";
            await SendDialogMessage(text, GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _inspectorPanel.ShowBuildingsList(category, asNewMessage: true)
                .ConfigureAwait(false);
        }

        public override async Task Start()
        {
            _inspectorPanel.OnDialogClose(); // чтобы убрать inline-клавиатуру, когда start вызван из "Назад к категориям"
            if (session.tooltipController.HasTooltipToAppend(this))
            {
                await SendHeader()
                    .ConfigureAwait(false);
                return;
            }
            await SendHeader()
                .ConfigureAwait(false);
            await SendPanelsAsync()
                .ConfigureAwait(false);
        }

        public async Task StartFromBuyResourcesDialog(BuildingBase inspectedBuilding, bool successfullPurchase)
        {
            await SendHeader().ConfigureAwait(false);
            if (successfullPurchase)
            {
                await new BuildingInfoDialog(session, inspectedBuilding).TryStartConstruction()
                    .ConfigureAwait(false);
                return;
            }
            await new BuildingInfoDialog(session, inspectedBuilding).Start()
                .ConfigureAwait(false);
        }

        public async Task StartWithShowBuildingInfo(BuildingBase building)
        {
            await SendHeader().ConfigureAwait(false);
            await new BuildingInfoDialog(session, building).Start()
                .ConfigureAwait(false);
        }

        private async Task SendHeader()
        {
            ClearButtons();
            RegisterCategoryButton(BuildingCategory.General);
            RegisterCategoryButton(BuildingCategory.Storages);
            RegisterCategoryButton(BuildingCategory.Production);
            RegisterCategoryButton(BuildingCategory.Training);

            RegisterTownButton(isFullBack: false);

            var sb = new StringBuilder();
            sb.Append($"{Emojis.menuItems[MenuItem.Buildings]} " + "<b>" + Localization.Get(session, "menu_item_buildings") + "</b>");
            TryAppendTooltip(sb);
            await SendDialogMessage(sb, GetKeyboardWithRowSizes(2, 2, 1))
                .ConfigureAwait(false);
        }

    }
}
