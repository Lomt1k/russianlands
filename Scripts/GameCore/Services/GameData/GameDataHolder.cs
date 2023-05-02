using System.IO;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    using Items;
    using System;
    using TextGameRPG.Scripts.GameCore.Buildings;
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

        public GameDataDictionary<BuildingId,BuildingData> buildings { get; private set; }
        public GameDataDictionary<int,ItemData> items { get; private set; }
        public GameDataDictionary<int,QuestMobData> mobs { get; private set; }
        public GameDataDictionary<int,PotionData> potions { get; private set; }
        public GameDataDictionary<QuestId, QuestData> quests { get; private set; }
        public GameDataDictionary<LocationId, LocationMobSettingsData> locationGeneratedMobs { get; private set; }

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

            buildings = LoadGameDataDictionary<BuildingId,BuildingData>("buildings");
            items = LoadGameDataDictionary<int,ItemData>("items");
            mobs = LoadGameDataDictionary<int,QuestMobData>("mobs");
            potions = LoadGameDataDictionary<int,PotionData>("potions");
            quests = LoadGameDataDictionary<QuestId, QuestData>("quests");
            locationGeneratedMobs = LoadGameDataDictionary<LocationId, LocationMobSettingsData>("locationGeneratedMobs");

            Localizations.Localization.LoadAll(_loader, gameDataPath);

            _loader?.OnGameDataLoaded();
            onDataReloaded?.Invoke();
        }

        private GameDataDictionary<TId, TData> LoadGameDataDictionary<TId, TData>(string fileName) where TData : IGameDataWithId<TId>
        {
            _loader?.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = GameDataDictionary<TId, TData>.LoadFromJSON<TId, TData>(fullPath);
            _loader?.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

        public void SaveAllData()
        {
            if (Program.appMode != AppMode.Editor)
                throw new InvalidOperationException("Game data can only be changed in Editor mode");

            buildings.Save();
            items.Save();
            mobs.Save();
            potions.Save();
            quests.Save();
            locationGeneratedMobs.Save();
        }

    }
}
