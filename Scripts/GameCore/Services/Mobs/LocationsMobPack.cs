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
        public Dictionary<LocationType, MobData[]> mobsByLocation { get; private set; } = new();

        public MobData[] this[LocationType locationType] => mobsByLocation[locationType];

        public LocationsMobPack(Dictionary<LocationType, MobData[]> _mobsByLocation)
        {
            mobsByLocation = _mobsByLocation;
        }
    }
}
