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
    public long dbid { get; set; }

    public ArenaBattleResult(BattleResult _result, long _dbid = 0)
    {
        result = _result;
        dbid = _dbid;
    }
}
