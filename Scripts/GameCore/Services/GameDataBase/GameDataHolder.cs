using System.IO;

namespace TextGameRPG.Scripts.GameCore.Services.GameData
{
    using Items;
    using TextGameRPG.Scripts.GameCore.Buildings.Data;
    using TextGameRPG.Scripts.GameCore.Potions;
    using TextGameRPG.Scripts.GameCore.Units.Mobs;
    using TextGameRPG.ViewModels;

    public class GameDataHolder : Service
    {
#pragma warning disable CS8618

        private const string gameDataPath = "gameData";

        private IGameDataLoader _loaderVM;

        public DataDictionaryWithIntegerID<BuildingData> buildings { get; private set; }
        public DataDictionaryWithIntegerID<ItemData> items { get; private set; }
        public DataDictionaryWithIntegerID<MobData> mobs { get; private set; }
        public DataDictionaryWithIntegerID<PotionData> potions { get; private set; }

#pragma warning restore CS8618

        public void LoadAllData(IGameDataLoader loaderVM)
        {
            _loaderVM = loaderVM;

            if (!Directory.Exists(gameDataPath))
            {
                Directory.CreateDirectory(gameDataPath);
                _loaderVM.AddNextState("'gameData' folder not found! Creating new gameData...");
            }

            buildings = LoadDataBaseWithIntegerID<BuildingData>("buildings");
            items = LoadDataBaseWithIntegerID<ItemData>("items");
            mobs = LoadDataBaseWithIntegerID<MobData>("mobs");
            potions = LoadDataBaseWithIntegerID<PotionData>("potions");
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
