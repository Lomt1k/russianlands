using System;
using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Buildings;

namespace TextGameRPG.Scripts.GameCore.Items.Generators
{
    public static class ItemGenerationHelper
    {
        private static readonly Dictionary<int, int> minItemLevelByTownHall;

        private static readonly Dictionary<int, int> basisPointsByTownHall = new Dictionary<int, int>
        {
            { 1, 20 },
            { 2, 20 },
            { 3, 30 },
            { 4, 40 },
            { 5, 50 },
            { 6, 70 },
            { 7, 100 },
            { 8, 140 },
            { 9, 180 },
            { 10, 220 },
        };

        private static readonly Dictionary<int, int[]> itemGradesByTownHall = new Dictionary<int, int[]>
        {
            { 1, new [] { 5 } },
            { 2, new [] { 3, 7 } },
            { 3, new [] { 2, 5, 8 } },
            { 4, new [] { 1, 4, 7, 10 } },
            { 5, new [] { 1, 3, 5, 7, 10 } },
            { 6, new [] { 1, 3, 4, 6, 8, 10 } },
            { 7, new [] { 1, 2, 3, 4, 6, 8, 10 } },
            { 8, new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
        };

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
            var grades = itemGradesByTownHall[townHallLevel];
            var index = new Random().Next(grades.Length);
            return grades[index];
        }

        public static int GetBasisPoint(int townHallLevel)
        {
            if (basisPointsByTownHall.TryGetValue(townHallLevel, out var basisPoint))
            {
                return basisPoint;
            }
            return townHallLevel < 1 ? basisPointsByTownHall[1] : basisPointsByTownHall[10];
        }

        public static int CalculateRequiredLevel(int townHallLevel, int grade)
        {
            var byTownHall = minItemLevelByTownHall[townHallLevel];
            var byGrade = GetAdditionalLevelsByGrade(townHallLevel, grade);
            return byTownHall + byGrade;
        }

        private static int GetAdditionalLevelsByGrade(int townHallLevel, int grade)
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
                    : grade == 5 || grade == 6 ? 4
                    : grade == 7 ? 5
                    : grade == 8 ? 6
                    : 7;
                case 8:            
                case 9:
                    return grade < 5 ? (grade - 1)
                        : grade == 5 || grade == 6 ? 4
                        : grade <= 8 ? 5 : 6;
            }

            return 0;
        }

    }
}
