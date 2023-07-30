using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Offers;
public class PremiumOfferDialog : DialogWithPanel
{
    public override DialogPanelBase DialogPanel => _panel;
    private PremiumOfferDialogPanel _panel;

    public PremiumOfferDialog(GameSession _session, PremiumOfferData offerData, OfferItem offerItem, Func<Task> onClose) : base(_session)
    {
        _panel = new PremiumOfferDialogPanel(this, offerData, offerItem);
        RegisterButton(Localization.Get(session, "offer_later_button"), onClose);
    }    

    public override async Task Start()
    {
        var header = Localization.Get(session, "offer_header").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _panel.Start().FastAwait();
    }
}
