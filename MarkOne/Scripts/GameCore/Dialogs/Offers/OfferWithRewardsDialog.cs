using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Offers;
public class OfferWithRewardsDialog : DialogWithPanel
{
    public override DialogPanelBase DialogPanel => _panel;
    private OfferWithRewardsDialogPanel _panel;

    public OfferWithRewardsDialog(GameSession _session, OfferWithRewardsData offerData, OfferItem offerItem, Func<Task> onClose) : base(_session)
    {
        _panel = new OfferWithRewardsDialogPanel(this, offerData, offerItem);
        RegisterButton(Localization.Get(session, "offer_later_button"), onClose);
    }

    public override async Task Start()
    {
        var header = Localization.Get(session, "offer_header").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _panel.Start().FastAwait();
    }
}
