using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.News;
internal class NewsDialog : DialogWithPanel
{
    private NewsDialogPanel _newsPanel;
    public override DialogPanelBase DialogPanel => _newsPanel;

    public NewsDialog(GameSession _session) : base(_session)
    {
        _newsPanel = new NewsDialogPanel(this);
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        var header = Localization.Get(session, "menu_item_news").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _newsPanel.Start().FastAwait();
    }
}
