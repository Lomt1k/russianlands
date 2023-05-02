﻿using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs;

[JsonObject]
public class CrossroadsMobData : IMobData
{
    public string localizationKey { get; set; } = string.Empty;
    public MobStatsSettings statsSettings { get; set; } = new();
    public List<MobAttack> mobAttacks { get; } = new();
    public ResourceId fruitId { get; set; }
}
