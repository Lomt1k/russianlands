using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.GameDataBase
{
    using Items.ItemGenerators;
    using TextGameRPG.ViewModels;

    public class GameDataBase
    {
        private const string itemGeneratorsPath = "gameData\\itemGenerators.json";

        public static GameDataBase instance;
        public DataDictionaryWithIntegerID<ItemGeneratorBase> itemsGenerators;

        public static void LoadAllData(GameDataLoaderViewModel? loaderVM = null)
        {
            instance = new GameDataBase();
            instance.LoadDataBases(loaderVM);
        }

        private void LoadDataBases(GameDataLoaderViewModel? loaderVM = null)
        {
            loaderVM?.AddNext("Loading Item Generators...");
            itemsGenerators = DataDictionaryWithIntegerID<ItemGeneratorBase>.LoadFromJSON<ItemGeneratorBase>(itemGeneratorsPath);
            loaderVM?.AddInfoToCurrentState( itemsGenerators.count.ToString() );

            loaderVM?.OnGameDataLoaded();
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
