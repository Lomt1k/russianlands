using System.IO;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    using Items;
    using System;
    using TextGameRPG.Scripts.GameCore.Buildings.Data;
    using TextGameRPG.Scripts.GameCore.Locations;
    using TextGameRPG.Scripts.GameCore.Potions;
    using TextGameRPG.Scripts.GameCore.Units.Mobs;
    using TextGameRPG.ViewModels;

    public class GameDataHolder : Service
    {
#pragma warning disable CS8618

        private static readonly string gameDataPath = Path.Combine("Assets", "gameData");

        private IGameDataLoader _loaderVM;

        public DataDictionaryWithIntegerID<BuildingData> buildings { get; private set; }
        public DataDictionaryWithIntegerID<ItemData> items { get; private set; }
        public DataDictionaryWithIntegerID<MobData> mobs { get; private set; }
        public DataDictionaryWithIntegerID<PotionData> potions { get; private set; }
        public DataDictionaryWithEnumID<LocationType, LocationMobData> locationGeneratedMobs { get; private set; }

#pragma warning restore CS8618

        public void LoadAllData(IGameDataLoader loaderVM)
        {
            _loaderVM = loaderVM;

            if (!Directory.Exists(gameDataPath))
            {
                Directory.CreateDirectory(gameDataPath);
                _loaderVM.AddNextState("'gameData' folder not found in Assets! Creating new gameData...");
            }

            buildings = LoadDataWithIntegerID<BuildingData>("buildings");
            items = LoadDataWithIntegerID<ItemData>("items");
            mobs = LoadDataWithIntegerID<MobData>("mobs");
            potions = LoadDataWithIntegerID<PotionData>("potions");
            locationGeneratedMobs = LoadDataWithEnumID<LocationType, LocationMobData>("locationGeneratedMobs");

            Localizations.Localization.LoadAll(_loaderVM, gameDataPath);
            Quests.QuestsHolder.LoadAll(_loaderVM, gameDataPath);

            _loaderVM.OnGameDataLoaded();
        }

        private DataDictionaryWithIntegerID<T> LoadDataWithIntegerID<T>(string fileName) where T : IDataWithIntegerID
        {
            _loaderVM.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = DataDictionaryWithIntegerID<T>.LoadFromJSON<T>(fullPath);
            _loaderVM.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

        private DataDictionaryWithEnumID<TEnum, TData> LoadDataWithEnumID<TEnum, TData>(string fileName) where TEnum : Enum where TData : IDataWithEnumID<TEnum>
        {
            _loaderVM.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = DataDictionaryWithEnumID<TEnum, TData>.LoadFromJSON<TEnum, TData>(fullPath);
            _loaderVM.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

    }
}
