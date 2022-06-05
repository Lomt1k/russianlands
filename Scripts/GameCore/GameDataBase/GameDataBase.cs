using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.GameDataBase
{
    using Items;
    using TextGameRPG.ViewModels;

    public class GameDataBase
    {
        private const string gameDataPath = "gameData";
        private static GameDataBase _instance;
        public static GameDataBase instance => _instance ??= new GameDataBase();

        private GameDataLoaderViewModel _loaderVM;
        
        public DataDictionaryWithIntegerID<ItemData> items { get; private set; }

        public void LoadAllData(GameDataLoaderViewModel loaderVM)
        {
            _loaderVM = loaderVM;
            items = LoadDataBaseWithIntegerID<ItemData>("items");
            Localizations.Localization.LoadAll(_loaderVM, gameDataPath);
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

        //private void TestWriteItemGenerators()
        //{
        //    var physicalDamageProp = new Items.ItemGenerators.ItemPropertyGenerators.PhysicalDamageItemPropertyGenerator(50, 100);
        //    var items = new List<ItemGeneratorBase>();
        //    for (int i = 0; i < 3; i++)
        //    {
        //        var itemGenerator = new ItemGeneratorBase("Тестовый меч", i, Items.ItemType.MeleeWeapon, Items.ItemRarity.Common, 10, physicalDamageProp);
        //        items.Add(itemGenerator);
        //    }

        //    var jsonStr = JsonConvert.SerializeObject(items, Formatting.Indented);

        //    var fileName = $"gameData\\itemGenerators.json";
        //    using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8))
        //    {
        //        writer.Write(jsonStr);
        //    }
        //}

    }
}
