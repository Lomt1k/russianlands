using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Dialogs.Town.Shop;
using MarkOne.Scripts.GameCore.Input;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Shop.Offers;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Offers;
public class PremiumOfferDialogPanel : DialogPanelBase
{
    private PremiumOfferData _offerData;
    private OfferItem _offerItem;

    public PremiumOfferDialogPanel(DialogWithPanel _dialog, PremiumOfferData offerData, OfferItem offerItem) : base(_dialog)
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
            sb.AppendLine();
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
        RegisterButton(Emojis.ElementInfo + Localization.Get(session, "offer_about_premium_button"), ShowPremiumAbout);

        if (string.IsNullOrWhiteSpace(_offerData.imageKey))
        {
            await SendPanelMessage(sb, GetMultilineKeyboard()).FastAwait();
        }
        else
        {
            var photo = InputFiles.Get(_offerData.imageKey);
            await SendPanelPhotoMessage(photo, sb, GetMultilineKeyboard()).FastAwait();
        }
    }

    private async Task ShowPremiumAbout()
    {
        var text = Localization.Get(session, "dialog_shop_premium_description", Emojis.ElementSmallBlack, Emojis.StatPremium,
            ShopDialogPanel.premiumDailyRewards[0].GetLocalizedView(session, showCountIfSingle: false),
            ShopDialogPanel.premiumDailyRewards[1].GetCompactView());

        ClearButtons();
        RegisterBackButton(Start);

        if (string.IsNullOrWhiteSpace(_offerData.imageKey))
        {
            await SendPanelMessage(text, GetOneLineKeyboard()).FastAwait();
        }
        else
        {
            await EditPhotoMessageCaption(text, GetOneLineKeyboard()).FastAwait();
        }
    }

}
