using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Dialogs.Town.Character;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Potions;

namespace TextGameRPG.Scripts.GameCore.Buildings.General
{
    public class AlchemyLabBuilding : BuildingBase
    {
        public override BuildingType buildingType => BuildingType.AlchemyLab;

        private static DataDictionaryWithIntegerID<PotionData> potionsDB => GameDataBase.GameDataBase.instance.potions;

        public override byte GetCurrentLevel(ProfileBuildingsData data)
        {
            return data.potionWorkshopLevel;
        }

        protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
        {
            data.potionWorkshopLevel = level;
        }

        protected override long GetStartConstructionTime(ProfileBuildingsData data)
        {
            return data.potionWorkshopStartConstructionTime;
        }

        protected override void SetStartConstructionTime(ProfileBuildingsData data, long startConstructionTime)
        {
            data.potionWorkshopStartConstructionTime = startConstructionTime;
        }

        public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, $"building_{buildingType}_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            var levelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel - 1];

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_types_of_potions_header"));            
            var potionTypesAmount = GetPotionsForCurrentLevel(data).Count;
            sb.Append($"{Emojis.menuItems[MenuItem.Potions]} {potionTypesAmount}");

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_potions_in_battle_header"));
            var potionsInBattle = levelInfo.potionsInBattle;
            sb.Append($"{Emojis.menuItems[MenuItem.Potions]} {potionsInBattle}");

            return sb.ToString();
        }

        public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
        {
            var sb = new StringBuilder();
            sb.Append(Localization.Get(session, $"building_{buildingType}_description"));

            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return sb.ToString();

            var currentLevelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel - 1];
            var nextLevelInfo = (AlchemyLabLevelInfo)buildingData.levels[currentLevel];

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_types_of_potions_header"));            
            var currentLevelAmount = GetPotionsForCurrentLevel(data).Count;
            var nextLevelAmount = GetPotionsForNextLevel(data).Count;
            var delta = nextLevelAmount - currentLevelAmount;
            sb.Append($"{Emojis.menuItems[MenuItem.Potions]} {nextLevelAmount}" + (delta > 0 ? $" (<i>+{delta}</i>)" : string.Empty) );

            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "building_potions_in_battle_header"));
            currentLevelAmount = currentLevelInfo.potionsInBattle;
            nextLevelAmount = nextLevelInfo.potionsInBattle;
            delta = nextLevelAmount - currentLevelAmount;
            sb.Append($"{Emojis.menuItems[MenuItem.Potions]} {nextLevelAmount}" + (delta > 0 ? $" (<i>+{delta}</i>)" : string.Empty) );

            return sb.ToString();
        }

        public List<PotionData> GetPotionsForCurrentLevel(ProfileBuildingsData data)
        {
            var result = new List<PotionData>();
            var currentLevel = GetCurrentLevel(data);
            if (currentLevel < 1)
                return result;

            foreach (var potionData in potionsDB.GetAllData())
            {
                if (potionData.workshopLevel == currentLevel)
                {
                    result.Add(potionData);
                }
            }
            return result;
        }

        public List<PotionData> GetPotionsForNextLevel(ProfileBuildingsData data)
        {
            var result = new List<PotionData>();
            var nextLevel = GetCurrentLevel(data) + 1;

            foreach (var potionData in potionsDB.GetAllData())
            {
                if (potionData.workshopLevel == nextLevel)
                {
                    result.Add(potionData);
                }
            }
            return result;
        }

        public override Dictionary<string, Func<Task>> GetSpecialButtons(GameSession session, ProfileBuildingsData data)
        {
            var result = new Dictionary<string, Func<Task>>();

            if (!IsUnderConstruction(data))
            {
                result.Add($"{Emojis.menuItems[MenuItem.Potions]} {Localization.Get(session, "menu_item_potions")}",
                    () => new PotionsDialog(session, backToBuilding: true).Start());
            }

            return result;
        }


    }
}
