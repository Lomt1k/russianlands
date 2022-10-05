using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public static class ItemGenerationHelper
    {
        private static readonly Dictionary<int, int> minItemLevelByTownHall;

        static ItemGenerationHelper()
        {
            minItemLevelByTownHall = new Dictionary<int, int>();
            var trainingBuilding = BuildingType.WarriorTraining.GetBuilding();
            var trainingBuildingLevels = trainingBuilding.buildingData.levels;

            minItemLevelByTownHall.Add(1, 1);
            minItemLevelByTownHall.Add(2, 1);
            foreach (var level in trainingBuildingLevels)
            {
                var trainingLevel = (Buildings.Data.TrainingLevelInfo)level;
                var townHallLevel = trainingLevel.requiredTownHall + 1;
                var requiredLevel = trainingLevel.maxUnitLevel + 1;
                minItemLevelByTownHall.Add(townHallLevel, requiredLevel);
            }
        }

        public static int GetRandomGrade(byte townHallLevel)
        {
            var random = new Random();
            int randValue = random.Next(100);
            switch (townHallLevel)
            {
                case 1:
                    return 5;
                case 2: // Ратуша 2: Равновероятное выпадение грейда 3 либо 7
                    return randValue < 50 ? 3 : 7;
                case 3: // Ратуша 3: 50% - 2, 30% - 5, 20% - 8
                    return randValue < 50 ? 2 : randValue < 80 ? 5 : 8;
                case 4: // Ратуша 4: 35% - 2, 30% - 4, 20% - 6, 15% - 8
                    return randValue < 35 ? 2 : randValue < 65 ? 4 : randValue < 85 ? 6 : 8;
                case 5: // Ратуша 5: 27% - 1, 23% - 3, 20% - 5, 17% - 7, 13% - 9
                    return randValue < 27 ? 1 : randValue < 50 ? 3 : randValue < 70 ? 5 : randValue < 87 ? 7 : 9;
                case 6: // Ратуша 6: 25% - 1, 21% - 3, 16% - 4, 16% - 6, 13% - 8, 9% - 10
                    return randValue < 25 ? 1 : randValue < 46 ? 3 : randValue < 62 ? 4
                        : randValue < 78 ? 6 : randValue < 91 ? 8 : 10;
                case 7: // Ратуша 7: 21% - 1, 18% - 2, 16% - 3, 14% - 4, 12% - 6, 11% - 8, 8% - 10
                    return randValue < 21 ? 1 : randValue < 39 ? 2 : randValue < 55 ? 3
                        : randValue < 69 ? 4 : randValue < 81 ? 6 : randValue < 92 ? 8 : 10;
            }
            return Randomizer.GetGrade();
        }

        public static int CalculateRequiredLevel(byte townHallLevel, int grade)
        {
            var byTownHall = minItemLevelByTownHall[townHallLevel];
            var byGrade = GetAdditionalLevelsByGrade(townHallLevel, grade);
            return byTownHall + byGrade;
        }

        private static int GetAdditionalLevelsByGrade(byte townHallLevel, int grade)
        {
            switch (townHallLevel)
            {
                case 2:
                    return grade <= 5 ? 0 : 2;
                case 3:
                    return grade < 5 ? 0 : grade < 8 ? 1 : 2;
                case 4:
                    return grade <= 5 ? 0 : grade <= 7 ? 1 : 2;
                case 5:
                    return grade <= 2 ? 0 : grade <= 4 ? 1 
                        : grade <= 6 ? 2 : grade <= 8 ? 3 : 4;
                case 6:
                    return grade <= 3 ? 0 : grade <= 5 ? 1 
                        : grade <= 7 ? 2 : grade <= 9 ? 3 : 4;
                case 7:
                case 10:
                    return grade < 5 ? (grade - 1)
                    : grade <= 6 ? 4
                    : grade == 7 ? 5
                    : grade == 8 ? 6
                    : 7;
                case 8:            
                case 9:
                    return grade <= 4 ? (grade - 1)
                        : grade <= 6 ? 4 : grade <= 8 ? 5 : 6;
            }

            return 0;
        }

    }
}
