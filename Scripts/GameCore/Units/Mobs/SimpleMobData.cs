using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs;

[JsonObject]
public class SimpleMobData : IMobData
{
    public string localizationKey { get; set; } = string.Empty;
    public MobStatsSettings statsSettings { get; set; } = new();
    public List<MobAttack> mobAttacks { get; } = new();
}
