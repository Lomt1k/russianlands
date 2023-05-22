using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public sealed class ShopArenaDialogPanel : DialogPanelBase
{
    private PlayerResources _playerResources => session.player.resources;

    public ShopArenaDialogPanel(DialogWithPanel _dialog) : base(_dialog)
    {
    }

    public override async Task Start()
    {
        await ShowCategories().FastAwait();
    }

    private async Task ShowCategories()
    {
        ClearButtons();
        RegisterButton(Localization.Get(session, "dialog_arena_shop_category_temporary"), null);
        RegisterButton(Localization.Get(session, "dialog_arena_shop_category_main"), null);
        var exchangeText = Localization.Get(session, "dialog_arena_shop_category_exchange") + Emojis.ResourceArenaTicket + " =>" + Emojis.ResourceArenaChip;
        RegisterButton(exchangeText, null);

        var resourcesToShow = new[]
        {
            _playerResources.GetResourceData(ResourceId.ArenaChip),
            _playerResources.GetResourceData(ResourceId.ArenaTicket),
        };        

        var sb = new StringBuilder()
            .AppendLine(Localization.Get(session, "resource_header_ours"))
            .AppendLine(resourcesToShow.GetCompactView(shortView: false))
            .AppendLine()
            .Append(Localization.Get(session, "dialog_arena_shop_select_category_header"));

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }

}
