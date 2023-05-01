using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Rewards;
using TextGameRPG.Scripts.GameCore.Services.GameData;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    [JsonObject]
    public class LocationMobData : IDataWithEnumID<LocationType>
    {
        [JsonProperty] public LocationType id { get; set; }
        [JsonProperty] public int mobsCount { get; set; }
        [JsonProperty] public Dictionary<byte, LocationMobDataByTownHall> dataByTownhall { get; set; } = new();

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

        public LocationMobDataByTownHall GetClosest(byte townhHallLevel)
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



            dataByTownhall.Add(newTownHall, new LocationMobDataByTownHall());
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
    public class LocationMobDataByTownHall
    {
        public int foodPrice { get; set; }
        public List<RewardBase> battleRewards { get; set; } = new List<RewardBase>();
        public List<RewardBase> locationRewards { get; set; } = new List<RewardBase>();
    }
}
