using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings
{
    public class BuildingsDialog : DialogBase
    {
        private BuildingsDialogPanel _inspectorPanel;

        public BuildingsDialog(GameSession _session) : base(_session)
        {
            _inspectorPanel = new BuildingsDialogPanel(this, 0);
            RegisterPanel(_inspectorPanel);
            RegisterTownButton(isDoubleBack: false);
        }

        public override async Task Start()
        {
            var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
            await SendDialogMessage(header, GetOneLineKeyboard())
                .ConfigureAwait(false);
            await SendPanelsAsync()
                .ConfigureAwait(false);
        }

        public async Task StartWithShowBuilding(BuildingBase building)
        {
            var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
            await SendDialogMessage(header, GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _inspectorPanel.ShowBuilding(building)
                .ConfigureAwait(false);
        }

        public async Task StartWithTryStartConstruction(BuildingBase building)
        {
            var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
            await SendDialogMessage(header, GetOneLineKeyboard())
                .ConfigureAwait(false);
            await _inspectorPanel.TryStartConstruction(building)
                    .ConfigureAwait(false);
        }

    }
}
