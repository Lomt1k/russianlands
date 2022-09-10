using System.IO;

namespace TextGameRPG.Scripts.GameCore.GameDataBase
{
    using Items;
    using TextGameRPG.Scripts.GameCore.Buildings.Data;
    using TextGameRPG.Scripts.GameCore.Locations;
    using TextGameRPG.Scripts.GameCore.Units.Mobs;
    using TextGameRPG.ViewModels;

    public class GameDataBase
    {
#pragma warning disable CS8618

        private const string gameDataPath = "gameData";

        private static GameDataBase _instance;

        public static GameDataBase instance => _instance ??= new GameDataBase();

        private GameDataLoaderViewModel _loaderVM;

        public DataDictionaryWithIntegerID<BuildingData> buildings { get; private set; }
        public DataDictionaryWithIntegerID<ItemData> items { get; private set; }
        public DataDictionaryWithIntegerID<LocationData> locations { get; private set; }
        public DataDictionaryWithIntegerID<MobData> mobs { get; private set; }

#pragma warning restore CS8618

        public void LoadAllData(GameDataLoaderViewModel loaderVM)
        {
            _loaderVM = loaderVM;

            if (!Directory.Exists(gameDataPath))
            {
                Directory.CreateDirectory(gameDataPath);
                _loaderVM.AddNextState("'gameData' folder not found! Creating new gameData...");
            }

            buildings = LoadDataBaseWithIntegerID<BuildingData>("buildings");
            items = LoadDataBaseWithIntegerID<ItemData>("items");
            locations = LoadDataBaseWithIntegerID<LocationData>("locations");
            mobs = LoadDataBaseWithIntegerID<MobData>("mobs");
            Localizations.Localization.LoadAll(_loaderVM, gameDataPath);
            Quests.QuestsHolder.LoadAll(_loaderVM, gameDataPath);
            _loaderVM.OnGameDataLoaded();
        }

        private DataDictionaryWithIntegerID<T> LoadDataBaseWithIntegerID<T>(string fileName) where T : IDataWithIntegerID
        {
            _loaderVM.AddNextState($"Loading {fileName}...");
            string fullPath = Path.Combine(gameDataPath, fileName + ".json");
            var dataBase = DataDictionaryWithIntegerID<T>.LoadFromJSON<T>(fullPath);
            _loaderVM.AddInfoToCurrentState(dataBase.count.ToString());
            return dataBase;
        }

    }
}
