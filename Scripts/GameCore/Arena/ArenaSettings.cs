using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaSettings : GameData
{
    public byte requiredTownhall { get; set; }
    public byte battlesInMatch { get; set; }
    public ArenaBattleRewardSettings battleRewardsForFood { get; set; } = new();
    public ArenaBattleRewardSettings battleRewardsForTicket { get; set; } = new();
    public List<ArenaTownhallSettings> townhallSettings { get; set; } = new();

    public ArenaTownhallSettings GetTownhallSettings(byte townhallLevel)
    {
        return townhallSettings.First(x => x.townhall == townhallLevel);
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
    public int ticketsForMatchEnd { get; set; }
}

[JsonObject]
public class ArenaTownhallSettings
{
    public byte townhall { get; set; }
    public int everydayFreeChips { get; set; }
    public int foodPrice { get; set; }
}

