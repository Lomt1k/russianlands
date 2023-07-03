using JsonKnownTypes;
using Newtonsoft.Json;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Potions;

[JsonObject]
[JsonConverter(typeof(JsonKnownTypesConverter<PotionData>))]
public abstract class PotionData : IGameDataWithId<int>
{
    public string debugName { get; set; } = "New Potion";
    //[IgnoreInEditor]
    public int id { get; set; }
    public byte workshopLevel { get; set; }
    public string localizationKey { get; set; } = string.Empty;
    public byte potionLevel { get; set; }

    public PotionData(int _id)
    {
        id = _id;
    }

    public void OnBotAppStarted()
    {
        // ignored
    }

    public string GetName(GameSession session)
    {
        return Emojis.ButtonPotions + Localization.Get(session, localizationKey)
            + $" {Localization.Get(session, "level_suffix", potionLevel)}";
    }

    public string GetNameWithoutIcon(GameSession session)
    {
        return Localization.Get(session, localizationKey)
            + $" {Localization.Get(session, "level_suffix", potionLevel)}";
    }

    public abstract string GetDescription(GameSession sessionForValues, GameSession sessionForView);
    public abstract void Apply(BattleTurn battleTurn, IBattleUnit unit);

}
