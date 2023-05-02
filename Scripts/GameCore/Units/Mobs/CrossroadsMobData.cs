using Newtonsoft.Json;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    [JsonObject]
    public class CrossroadsMobData : MobData
    {
        public ResourceId fruitId { get; set; }
    }
}
