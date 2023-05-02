using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Localizations;

namespace MarkOne.Scripts.Bot.Dialogs.Town.Buildings;

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
