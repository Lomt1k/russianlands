using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Services.DailyDataManagers;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public class LocationMobsManager : Service
    {
        public const int MOBS_ON_LOCATION = 3;

        private static readonly ServerDailyDataManager serverDailyDataManager = Services.Get<ServerDailyDataManager>();
        private static readonly MobFactory mobFactory = Services.Get<MobFactory>();

        private Dictionary<MobDifficulty, LocationsMobPack> _mobPacks = new();

        public LocationsMobPack this[MobDifficulty mobDifficulty] => _mobPacks[mobDifficulty];

        public bool isMobsReady { get; private set; }

        public LocationMobsManager()
        {
            serverDailyDataManager.onStartWithOldDay += OnStartWithOldDay;
            serverDailyDataManager.onStartNewDay += OnStartNewDay;
        }

        private void OnStartWithOldDay()
        {
            isMobsReady = false;
            LoadMobPacks();
        }

        private void OnStartNewDay(DateTime oldDate, DateTime newDate)
        {
            isMobsReady = false;
            CreateNewMobPacks();
        }

        private async void LoadMobPacks()
        {
            var jsonStr = await serverDailyDataManager.GetStringValue("locationMobs", string.Empty).FastAwait();
            if (string.IsNullOrEmpty(jsonStr))
            {
                CreateNewMobPacks();
                return;
            }
            var loadedMobPacks = JsonConvert.DeserializeObject<Dictionary<MobDifficulty, LocationsMobPack>>(jsonStr);
            if (loadedMobPacks is null)
            {
                Program.logger.Error("LocationMobsManager: Error when try to desealize 'locationMobs'. Creating new mobs...");
                CreateNewMobPacks();
                return;
            }
            _mobPacks = loadedMobPacks;
            Program.logger.Info("LocationMobsManager: Location mobs loaded successfully");
            isMobsReady = true;
        }

        private async void CreateNewMobPacks()
        {
            _mobPacks.Clear();
            foreach (MobDifficulty difficulty in Enum.GetValues(typeof(MobDifficulty)))
            {
                _mobPacks[difficulty] = GenerateMobPack(difficulty);
            }
            var jsonStr = JsonConvert.SerializeObject(_mobPacks);
            await serverDailyDataManager.SetStringValue("locationMobs", jsonStr).FastAwait();
            Program.logger.Info("LocationMobsManager: New location mobs created");
            isMobsReady = true;
        }

        private LocationsMobPack GenerateMobPack(MobDifficulty difficulty)
        {
            var result = new Dictionary<LocationType, MobData[]>();
            foreach (LocationType locationType in Enum.GetValues(typeof(LocationType)))
            {
                if (locationType == LocationType.None)
                    continue;

                var array = new MobData[MOBS_ON_LOCATION];
                for (int i = 0; i < MOBS_ON_LOCATION; i++)
                {
                    array[i] = mobFactory.GenerateMobForLocation(difficulty, locationType);
                }
                result[locationType] = array;
            }
            return new LocationsMobPack(result);
        }

    }
}
