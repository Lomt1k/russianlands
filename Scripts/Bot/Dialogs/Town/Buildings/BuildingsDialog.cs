using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Localizations;

namespace TextGameRPG.Scripts.Bot.Dialogs.Town.Buildings;

public class BuildingsDialog : DialogWithPanel
{
    private readonly BuildingsDialogPanel _inspectorPanel;
    public override DialogPanelBase DialogPanel => _inspectorPanel;

    public BuildingsDialog(GameSession _session) : base(_session)
    {
        _inspectorPanel = new BuildingsDialogPanel(this);
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _inspectorPanel.Start().FastAwait();
    }

    public async Task StartWithShowBuilding(BuildingBase building)
    {
        var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _inspectorPanel.ShowBuilding(building).FastAwait();
    }

    public async Task StartWithTryStartConstruction(BuildingBase building)
    {
        var header = Emojis.ButtonBuildings + Localization.Get(session, "menu_item_buildings").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _inspectorPanel.TryStartConstruction(building).FastAwait();
    }

}
