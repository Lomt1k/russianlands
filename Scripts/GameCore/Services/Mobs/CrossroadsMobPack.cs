using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    [JsonObject]
    public class CrossroadsMobPack
    {
        public const int MOB_SETS_IN_ONE_PACK = 50;

        [JsonProperty]
        private Dictionary<int, CrossroadsMobData[]> mobsByCrossId { get; } = new();

        public MobData[] this[int crossId]
        {
            get
            {
                var setId = (crossId - 1) % MOB_SETS_IN_ONE_PACK;
                return mobsByCrossId[setId];
            }
        }

        public CrossroadsMobPack(Dictionary<int, CrossroadsMobData[]> _mobsByLocation)
        {
            mobsByCrossId = _mobsByLocation;
        }
    }
}
