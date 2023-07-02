using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Services.GameData;

namespace MarkOne.Scripts.GameCore.Units.Mobs;

[JsonObject]
public class QuestMobData : IMobData, IGameDataWithId<int>
{
    public int id { get; set; }
    public string debugName { get; set; } = "New Mob";
    public string localizationKey { get; set; } = string.Empty;
    public MobStatsSettings statsSettings { get; set; } = new();
    public List<MobAttack> mobAttacks { get; } = new();

    public void OnSetupAppMode(AppMode appMode)
    {
        if (appMode == AppMode.PlayMode)
        {
            debugName = string.Empty;
        }
    }
}
