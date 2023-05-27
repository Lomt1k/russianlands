using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public sealed class ShopArenaDialog : DialogWithPanel
{
    private ShopArenaDialogPanel _shopPanel;
    public override DialogPanelBase DialogPanel => _shopPanel;

    public ShopArenaDialog(GameSession _session) : base(_session)
    {
        _shopPanel = new(this);
        RegisterBackButton(Localization.Get(session, "menu_item_arena") + Emojis.ButtonArena, () => new ArenaDialog(session).Start());
        RegisterDoubleBackButton(Localization.Get(session, "menu_item_map") + Emojis.ButtonMap, () => new MapDialog(session).Start());
    }

    public override async Task Start()
    {
        var header = Emojis.ElementScales + Localization.Get(session, "dialog_arena_shop_header");
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _shopPanel.Start().FastAwait();
    }

    public async Task StartWithCategory(ShopArenaCategory category)
    {
        var header = Emojis.ElementScales + Localization.Get(session, "dialog_arena_shop_header");
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _shopPanel.ShowCategory(category).FastAwait();
    }
}
