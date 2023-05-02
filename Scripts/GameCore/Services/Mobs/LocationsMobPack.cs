using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Units.Mobs;

namespace MarkOne.Scripts.GameCore.Services.Mobs;

[JsonObject]
public class LocationsMobPack
{
    [JsonProperty]
    private Dictionary<LocationId, SimpleMobData[]> mobsByLocation { get; } = new();

    public SimpleMobData[] this[LocationId locationId] => mobsByLocation[locationId];

    public LocationsMobPack(Dictionary<LocationId, SimpleMobData[]> _mobsByLocation)
    {
        mobsByLocation = _mobsByLocation;
    }
}
