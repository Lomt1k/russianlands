using System.Threading.Tasks;
using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map;

public class MapDialog : DialogWithPanel
{
    private static readonly int crossroadRequiredTownhall = BuildingId.ElixirWorkshop.GetBuilding().buildingData.levels[0].requiredTownHall;
    private static readonly int arenaRequiredTownhall = gameDataHolder.arenaSettings.requiredTownhall;

    private readonly MapDialogPanel _mapPanel;
    public override DialogPanelBase DialogPanel => _mapPanel;


    public MapDialog(GameSession _session) : base(_session)
    {
        _mapPanel = new MapDialogPanel(this);

        RegisterButton(Emojis.ButtonArena + Localization.Get(session, "menu_item_arena")
            + (ArenaDialog.CanCollectFreeChips(session) ? Emojis.ElementWarningRed.ToString() : string.Empty), TryOpenArena);
        RegisterButton(Emojis.ButtonCrossroads + Localization.Get(session, "menu_item_crossroads"), TryOpenCrossroads);
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        var header = Emojis.ButtonMap + Localization.Get(session, "menu_item_map").Bold();
        await SendDialogMessage(header, GetKeyboardWithRowSizes(2, 1)).FastAwait();
        await _mapPanel.Start().FastAwait();
    }

    public async Task StartWithLocation(LocationId locationId)
    {
        var header = Emojis.ButtonMap + Localization.Get(session, "menu_item_map").Bold();
        await SendDialogMessage(header, GetKeyboardWithRowSizes(2, 1)).FastAwait();
        await _mapPanel.ShowLocation(locationId).FastAwait();
    }

    private async Task TryOpenArena()
    {
        var townhallLevel = session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
        if (townhallLevel < arenaRequiredTownhall)
        {
            var notification = Emojis.ButtonArena + Localization.Get(session, "dialog_map_arena_is_locked", arenaRequiredTownhall);
            var photo = InputFile.FromFileId(Localization.Get(session, "photo_fileId_loc_arena"));
            await notificationsManager.ShowNotification(session, photo, notification, () => new MapDialog(session).Start()).FastAwait();
            return;
        }
        await new ArenaDialog(session).Start().FastAwait();
    }

    private async Task TryOpenCrossroads()
    {
        var townhallLevel = session.player.buildings.GetBuildingLevel(BuildingId.TownHall);
        if (townhallLevel < crossroadRequiredTownhall)
        {
            var notification = Emojis.ButtonCrossroads + Localization.Get(session, "dialog_map_crossroads_is_locked", crossroadRequiredTownhall);
            var photo = InputFile.FromFileId(Localization.Get(session, "photo_fileId_loc_crossroads"));
            await notificationsManager.ShowNotification(session, photo, notification, () => new MapDialog(session).Start()).FastAwait();
            return;
        }
        await new Crossroads.CrossroadsDialog(session).Start().FastAwait();
    }

    public static bool HasNewActivities(GameSession session)
    {
        return ArenaDialog.CanCollectFreeChips(session);
    }

}
