using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Services.Payments;
using MarkOne.Scripts.GameCore.Sessions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Shop.Offers;
[JsonObject]
public class OfferItem
{
    private static readonly GameDataHolder gameDataBase = Services.ServiceLocator.Get<GameDataHolder>();
    private static readonly PaymentManager paymentManager = Services.ServiceLocator.Get<PaymentManager>();

    [JsonProperty("id")]
    private readonly int _id;
    [JsonProperty("st")]
    private long _lastStartTime;
    [JsonProperty("et")]
    private long _lastEndTime;
    [JsonProperty("c")]
    private ushort _activationsCount;
    [JsonProperty("oid")]
    private string _orderId;

    [JsonIgnore]
    public int id => _id;
    [JsonIgnore]
    public DateTime lastStartTime => new DateTime(_lastStartTime);
    [JsonIgnore]
    public DateTime lastEndTime => new DateTime(_lastEndTime);
    [JsonIgnore]
    public int activationsCount => _activationsCount;
    [JsonIgnore]
    public string orderId => _orderId;

    public OfferItem(int id)
    {
        _id = id;
    }

    public async Task<bool> TryActivate(GameSession session)
    {
        var data = GetData();
        var dtNow = DateTime.UtcNow;
        var offerEndTime = dtNow.AddHours(GetData().activityHours);
        var paymentEndTime = offerEndTime.AddMinutes(15);
        var comment = data.GetTitle(session);
        var paymentData = await paymentManager.TryGetOrCreatePayment(session, data.priceRubles, data.vendorCode, comment, paymentEndTime).FastAwait();
        if (paymentData is null)
        {
            return false;
        }

        _lastStartTime = dtNow.Ticks;
        _lastEndTime = offerEndTime.Ticks;
        _activationsCount++;
        _orderId = paymentData.orderId;
        return true;
    }

    public OfferData GetData()
    {
        return gameDataBase.offers[_id];
    }

    public bool IsActive()
    {
        return lastEndTime > DateTime.UtcNow;
    }

    public TimeSpan GetTimeToEnd()
    {
        return lastEndTime - DateTime.UtcNow;
    }

    public string GetTitleForList(GameSession session)
    {
        var timeToEnd = GetTimeToEnd();
        return $"{timeToEnd.GetShortViewFloor(session)} | {GetData().GetTitle(session)}";
    }

    public bool IsActivationsLimitReached()
    {
        return _activationsCount >= GetData().activationsLimit;
    }

    public bool IsCooldownRequired()
    {
        var endCooldownTime = lastEndTime.AddDays(GetData().cooldownInDays);
        return endCooldownTime > DateTime.UtcNow;
    }

    public void Deactivate()
    {
        _lastEndTime = DateTime.UtcNow.Ticks;
    }
    
}
