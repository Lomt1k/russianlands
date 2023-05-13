using MarkOne.Scripts.GameCore.Services.GameData;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class ArenaSettings : GameData
{
    public byte requiredTownhall { get; set; }
    public byte battlesInMatch { get; set; }
    public int chipsForMatchEnd { get; set; }
    public int chipsForBattleWin { get; set; }
    public int chipsForBattleDraw { get; set; }
    public Dictionary<byte, ArenaTownhallSettings> townhallSettings { get; set; } = new();

    protected override void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }
}

[JsonObject]
public class ArenaTownhallSettings
{
    public int everydayFreeChips { get; set; }
    public int foodPrice { get; set; }
}

