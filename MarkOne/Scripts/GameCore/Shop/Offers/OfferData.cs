using JsonKnownTypes;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<OfferData>))]
public abstract class OfferData : IGameDataWithId<int>
{
    public int id { get; set; }
    public string comment { get; set; } = string.Empty;
    public bool isEnabled { get; set; }
    public string titleKey { get; set; } = string.Empty;
    public string descriptionKey { get; set; } = string.Empty;
    public string bestBuyKey { get; set; } = string.Empty;
    public string imageKey { get; set; } = string.Empty;
    public uint visualPriceWithoutOffer { get; set; }
    public uint priceRubles { get; set; }
    public int activityHours { get; set; }
    public int cooldownInDays { get; set; }
    public int activationsLimit { get; set; } = 1;
    public int priority { get; set; } = 100;

    public string vendorCode => $"offer-{id}";

    public OfferData(int _id)
    {
        id = _id;
    }

    public abstract string GetTitle(GameSession session);
    public abstract string GetDescription(GameSession session);
    public abstract string GetBestBuyLabel(GameSession session);
    public abstract Task StartOfferDialog(GameSession session, OfferItem offerItem, Func<Task> onClose);
    public abstract ShopItemBase GenerateShopItem();

    public string GetTimeToEndLabel(GameSession session, OfferItem offerItem)
    {
        var timeToEnd = offerItem.lastEndTime - DateTime.UtcNow;
        return Localization.Get(session, "offer_end_time_label", timeToEnd.GetView(session));
    }

    public string GetPriceView()
    {
        return $"{priceRubles} ₽";
    }

    public string GetPriceWithoutOfferView()
    {
        return $"{visualPriceWithoutOffer} ₽";
    }

    public void OnBotAppStarted()
    {
        // ignored
    }

}
