using Newtonsoft.Json;
using System;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Potions;

[JsonObject]
public class PotionItem
{
    private static readonly GameDataHolder gameDataBase = Services.ServiceLocator.Get<GameDataHolder>();


    [JsonProperty("id")]
    private readonly int _id;
    [JsonProperty("t")]
    private long _preparationTime;

    [JsonIgnore]
    public int id => _id;
    [JsonIgnore]
    public long preparationTime => _preparationTime;

    public PotionItem(int id, long preparationTime)
    {
        _id = id;
        _preparationTime = preparationTime;
    }

    public PotionData GetData()
    {
        return gameDataBase.potions[_id];
    }

    public bool IsReady()
    {
        return DateTime.UtcNow.Ticks > _preparationTime;
    }

    public void BoostProduction()
    {
        _preparationTime = DateTime.UtcNow.Ticks - 1;
    }

    public ResourceData GetBoostPriceInDiamonds()
    {
        if (IsReady())
        {
            return new ResourceData(ResourceId.Diamond, 0);
        }

        var timeSpan = new DateTime(preparationTime) - DateTime.UtcNow;
        var seconds = (int)timeSpan.TotalSeconds;
        return ResourceHelper.CalculatePotionCraftBoostPriceInDiamonds(seconds);
    }

    public string GetName(GameSession session)
    {
        return GetData().GetName(session);
    }

    public string GetNameForList(GameSession session)
    {
        if (IsReady())
        {
            return GetData().GetName(session);
        }

        var timeSpan = new DateTime(_preparationTime) - DateTime.UtcNow;
        return timeSpan.GetShortView(session) + " | " + GetData().GetNameWithoutIcon(session);
    }

    public string GetView(GameSession session)
    {
        var sb = new StringBuilder();
        var data = GetData();
        sb.AppendLine(data.GetName(session).Bold());
        sb.AppendLine();
        sb.AppendLine(data.GetDescription(session, session));

        if (!IsReady())
        {
            sb.AppendLine();
            var timeSpan = new DateTime(_preparationTime) - DateTime.UtcNow;
            var productionView = Localization.Get(session, "dialog_potions_production_progress", timeSpan.GetView(session));
            sb.Append(Emojis.ElementSmallBlack + productionView);
        }
        return sb.ToString();
    }

}
