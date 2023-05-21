using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaSettings : GameData
{
    public byte requiredTownhall { get; set; }
    public byte battlesInMatch { get; set; }
    public ArenaBattleRewardSettings battleRewardsForFood { get; set; } = new();
    public ArenaBattleRewardSettings battleRewardsForTicket { get; set; } = new();
    public List<ArenaTownhallSettings> townhallSettings { get; set; } = new();

    [JsonIgnore]
    private Dictionary<byte, ArenaTownhallSettings> townhallSettingsByLevel { get; set; } = new();

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        townhallSettingsByLevel = townhallSettings.ToDictionary(x => x.townhall);
    }

    public ArenaTownhallSettings GetTownhallSettings(byte townhallLevel)
    {
        return townhallSettingsByLevel[townhallLevel];
    }

    protected override void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }
}

[JsonObject]
public class ArenaBattleRewardSettings
{
    public int chipsForBattleWin { get; set; }
    public int chipsForBattleDraw { get; set; }
    public int chipsForMatchEnd { get; set; }
    public int ticketsForWinAllBattles { get; set; }
    public int chipsForWinAllBattles { get; set; }
    public int chipsForDrawAllBattles { get; set; }
}

[JsonObject]
public class ArenaTownhallSettings
{
    public byte townhall { get; set; }
    public int everydayFreeChips { get; set; }
    public int foodPrice { get; set; }
}

