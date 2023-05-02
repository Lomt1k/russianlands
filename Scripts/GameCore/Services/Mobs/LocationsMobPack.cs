using Newtonsoft.Json;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    [JsonObject]
    public class LocationsMobPack
    {
        [JsonProperty]
        private Dictionary<LocationId, MobData[]> mobsByLocation { get; } = new();

        public MobData[] this[LocationId locationId] => mobsByLocation[locationId];

        public LocationsMobPack(Dictionary<LocationId, MobData[]> _mobsByLocation)
        {
            mobsByLocation = _mobsByLocation;
        }
    }
}
