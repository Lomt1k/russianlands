using JsonKnownTypes;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<OfferData>))]
public abstract class OfferData : IGameDataWithId<int>
{
    public int id { get; set; }
    public string comment { get; set; } = string.Empty;
    public bool isEnabled { get; set; }
    public string vendorCode { get; set; } = string.Empty;
    public string titleKey { get; set; } = string.Empty;
    public string descriptionKey { get; set; } = string.Empty;
    public uint visualPriceWithoutOffer { get; set; }
    public uint priceRubles { get; set; }
    public int activityHours { get; set; }
    public int cooldownInDays { get; set; }
    public bool IsOneTimeOffer { get; set; }

    public OfferData(int _id)
    {
        id = _id;
    }

    public abstract string GetTitle(GameSession session);
    public abstract Task StartOfferDialog(GameSession session);

    public void OnBotAppStarted()
    {
        // ignored
    }

}
