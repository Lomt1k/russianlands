using System.IO;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    using Items;
    using System;
    using TextGameRPG.Scripts.GameCore.Buildings.Data;
    using TextGameRPG.Scripts.GameCore.Locations;
    using TextGameRPG.Scripts.GameCore.Potions;
    using TextGameRPG.Scripts.GameCore.Quests;
    using TextGameRPG.Scripts.GameCore.Units.Mobs;
    using TextGameRPG.ViewModels;

    public class GameDataHolder : Service
    {
#pragma warning disable CS8618

        private static readonly string gameDataPath = Path.Combine("Assets", "gameData");

        private GameDataLoader? _loader;

        public DataDictionaryWithIntegerID<BuildingData> buildings { get; private set; }
        public DataDictionaryWithIntegerID<ItemData> items { get; private set; }
        public DataDictionaryWithIntegerID<MobData> mobs { get; private set; }
        public DataDictionaryWithIntegerID<PotionData> potions { get; private set; }
        public DataDictionaryWithEnumID<QuestId, QuestData> quests { get; private set; }
        public DataDictionaryWithEnumID<LocationId, LocationMobData> locationGeneratedMobs { get; private set; }

#pragma warning restore CS8618

        public event Action? onDataReloaded;

        public void LoadAllData(GameDataLoader? loader = null)
        {
            _loader = loader;

            if (!Directory.Exists(gameDataPath))
            {
                Directory.CreateDirectory(gameDataPath);
                _loader?.AddNextState("'gameData' folder not found in Assets! Creating new gameData...");
            }

            buildings = LoadDataWithIntegerID<BuildingData>("buildings");
            items = LoadDataWithIntegerID<ItemData>("items");
            mobs = LoadDataWithIntegerID<MobData>("mobs");
            potions = LoadDataWithIntegerID<PotionData>("potions");
            quests = LoadDataWithEnumID<QuestId, QuestData>("quests");
            locationGeneratedMobs = LoadDataWithEnumID<LocationId, LocationMobData>("locationGeneratedMobs");

            Localizations.Localization.LoadAll(_loader, gameDataPath);

            _loader?.OnGameDataLoaded();
            onDataReloaded?.Invoke();
        }

        private DataDictionaryWithIntegerID<T> LoadDataWithIntegerID<T>(string fileName) where T : IDataWithIntegerID
        {
            _loader?.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = DataDictionaryWithIntegerID<T>.LoadFromJSON<T>(fullPath);
            _loader?.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

        private DataDictionaryWithEnumID<TEnum, TData> LoadDataWithEnumID<TEnum, TData>(string fileName) where TEnum : Enum where TData : IDataWithEnumID<TEnum>
        {
            _loader?.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = DataDictionaryWithEnumID<TEnum, TData>.LoadFromJSON<TEnum, TData>(fullPath);
            _loader?.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

        public void SaveAllData()
        {
            if (Program.appMode != AppMode.Editor)
                return;

            buildings.Save();
            items.Save();
            mobs.Save();
            potions.Save();
            quests.Save();
            locationGeneratedMobs.Save();
        }

    }
}
