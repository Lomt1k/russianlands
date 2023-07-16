using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Services.Payments;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Shop;

public class ShopDialog : DialogWithPanel
{
    private ShopDialogPanel _shopPanel;
    public override DialogPanelBase DialogPanel => _shopPanel;

    public ShopDialog(GameSession _session) : base(_session)
    {
        _shopPanel = new(this);
        RegisterTownButton(isDoubleBack: false);
    }

    public override async Task Start()
    {
        var header = Emojis.ButtonShop + Localization.Get(session, "menu_item_shop").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _shopPanel.Start().FastAwait();
    }

    public async Task StartWithCategory(ShopCategory category)
    {
        var header = Emojis.ButtonShop + Localization.Get(session, "menu_item_shop").Bold();
        await SendDialogMessage(header, GetOneLineKeyboard()).FastAwait();
        await _shopPanel.ShowCategory(category).FastAwait();
    }

    public async Task ShowPaymentMessage(PaymentData paymentData, ShopItemBase shopItem)
    {
        await _shopPanel.ShowPaymentMessage(paymentData, shopItem).FastAwait();
    }
}
