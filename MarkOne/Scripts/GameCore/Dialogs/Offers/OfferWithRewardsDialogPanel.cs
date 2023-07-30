using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Offers;
public class OfferWithRewardsDialogPanel : DialogPanelBase
{
    private OfferWithRewardsData _offerData;
    private OfferItem _offerItem;

    public OfferWithRewardsDialogPanel(DialogWithPanel _dialog, OfferWithRewardsData offerData, OfferItem offerItem) : base(_dialog)
    {
        _offerData = offerData;
        _offerItem = offerItem;
    }

    public override async Task Start()
    {
        var sb = new StringBuilder()
            .AppendLine(_offerData.GetTitle(session).Bold())
            .AppendLine(_offerData.GetBestBuyLabel(session).CodeBlock())
            .AppendLine()
            .AppendLine(_offerData.GetDescription(session));

        var oldPriceView = _offerData.GetPriceWithoutOfferView();
        var priceView = _offerData.GetPriceView();
        if (_offerData.visualPriceWithoutOffer > 0)
        {
            sb.AppendLine(Localization.Get(session, "offer_price_without_offer", oldPriceView));
            sb.AppendLine(Localization.Get(session, "offer_price_with_offer", priceView));
        }

        sb.AppendLine();
        sb.AppendLine(_offerData.GetTimeToEndLabel(session, _offerItem));

        var orderId = _offerItem.orderId;
        var db = BotController.dataBase.db;
        var paymentData = db.Table<PaymentData>().Where(x => x.orderId == orderId).FirstOrDefault();
        if (paymentData is null)
        {
            return;
        }


        ClearButtons();
        RegisterLinkButton(Localization.Get(session, "menu_item_buy_button", priceView), paymentData.url);

        await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
    }
}
