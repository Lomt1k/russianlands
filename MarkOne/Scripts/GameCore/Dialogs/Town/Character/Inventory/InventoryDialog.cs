using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Dialogs.Town.Character.Inventory;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Character.Inventory;

public class InventoryDialog : DialogWithPanel
{
    private readonly InventoryInspectorDialogPanel _inspectorPanel;
    public override DialogPanelBase DialogPanel => _inspectorPanel;

    public InventoryDialog(GameSession _session) : base(_session)
    {
        _inspectorPanel = new InventoryInspectorDialogPanel(this);
        RegisterBackButton(Localization.Get(session, "menu_item_character") + session.player.avatarId.GetEmoji(),
            () => new CharacterDialog(session).Start());
        RegisterTownButton(isDoubleBack: true);
    }

    public override async Task Start()
    {
        var header = Emojis.ButtonInventory + Localization.Get(session, "menu_item_inventory").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _inspectorPanel.Start().FastAwait();
    }

}
