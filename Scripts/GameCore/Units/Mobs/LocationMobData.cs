using Newtonsoft.Json;
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
        public LocationType id { get; set; }
        public Dictionary<byte, LocationMobDataByTownHall> dataByTownhall { get; set; }

        [JsonIgnore]
        public byte minTownHall { get; private set; }
        [JsonIgnore]
        public byte maxTownHall { get; private set; }

        public void OnSetupAppMode(AppMode appMode)
        {
            // ignored
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            minTownHall = dataByTownhall.Keys.Min();
            maxTownHall = dataByTownhall.Keys.Max();
        }

        public LocationMobDataByTownHall Get(byte townhHallLevel)
        {
            if (dataByTownhall.TryGetValue(townhHallLevel, out var value))
            {
                return value;
            }
            return townhHallLevel < minTownHall ? dataByTownhall[minTownHall] : dataByTownhall[maxTownHall];
        }
    }

    [JsonObject]
    public class LocationMobDataByTownHall
    {
        public int mobsCount { get; set; }
        public int foodPrice { get; set; }
        public List<RewardBase> battleRewards { get; set; } = new List<RewardBase>();
        public List<RewardBase> locationRewards { get; set; } = new List<RewardBase>();
    }
}
