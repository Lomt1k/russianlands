using MarkOne.Scripts.GameCore.Dialogs.Battle;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Arena;
[JsonObject]
public class PlayerArenaProgress
{
    public bool byTicket { get; set; }
    public List<ArenaBattleResult> results { get; set; } = new();
}

[JsonObject]
public class ArenaBattleResult
{
    public BattleResult result { get; set; }
    public string name { get; set; } = string.Empty;
}
