using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Services.GameData;

namespace MarkOne.Scripts.GameCore.Units.Mobs;

[JsonObject]
public class LocationMobSettingsData : IGameDataWithId<LocationId>
{
    [JsonProperty] public LocationId id { get; set; }
    [JsonProperty] public int mobsCount { get; set; }
    [JsonProperty] public Dictionary<byte, LocationMobSettingsDataByTownHall> dataByTownhall { get; set; } = new();

    [JsonIgnore] public byte minTownHall { get; private set; }
    [JsonIgnore] public byte maxTownHall { get; private set; }

    [OnDeserialized]
    internal void OnDeserialized(StreamingContext context)
    {
        OnUpdateDictionary();
    }

    public void OnSetupAppMode(AppMode appMode)
    {
        // ignored
    }

    public LocationMobSettingsDataByTownHall GetClosest(byte townhHallLevel)
    {
        if (dataByTownhall.TryGetValue(townhHallLevel, out var value))
        {
            return value;
        }
        return townhHallLevel < minTownHall ? dataByTownhall[minTownHall] : dataByTownhall[maxTownHall];
    }

    public byte AddNewTownHall()
    {
        if (Program.appMode != AppMode.Editor)
        {
            throw new InvalidOperationException("You can only change the date in editor mode");
        }
        var newTownHall = (byte)(maxTownHall + 1);



        dataByTownhall.Add(newTownHall, new LocationMobSettingsDataByTownHall());
        OnUpdateDictionary();
        return newTownHall;
    }

    public void RemoveTownHall(byte townHall)
    {
        if (Program.appMode != AppMode.Editor)
        {
            throw new InvalidOperationException("You can only change the date in editor mode");
        }
        dataByTownhall.Remove(townHall);
        OnUpdateDictionary();
    }

    private void OnUpdateDictionary()
    {
        minTownHall = dataByTownhall.Keys.Count > 0 ? dataByTownhall.Keys.Min() : (byte)0;
        maxTownHall = dataByTownhall.Keys.Count > 0 ? dataByTownhall.Keys.Max() : (byte)0;
    }
}

[JsonObject]
public class LocationMobSettingsDataByTownHall
{
    public int foodPrice { get; set; }
    public List<RewardBase> battleRewards { get; set; } = new List<RewardBase>();
    public List<RewardBase> locationRewards { get; set; } = new List<RewardBase>();
}
