using System;
using System.Collections.Generic;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Resources;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public class MobFactory : Service
    {
        private static readonly LocationId[] crossroadLocationNames =
        {
            LocationId.Loc_01,
            LocationId.Loc_02,
            LocationId.Loc_03,
            LocationId.Loc_04,
        };

        private static readonly ResourceId[] crossroadFruits = ResourcesDictionary.GetFruitTypes().ToArray();

        public MobData GenerateMobForDebugBattle(byte playerLevel)
        {
            return new MobDataBuilder(playerLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(playerLevel - 1, playerLevel + 1)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(playerLevel - 1, playerLevel + 1)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName()
                .GetResult();
        }

        public MobData GenerateMobForLocation(MobDifficulty mobDifficulty, LocationId locationId, List<string> excludeNames)
        {
            if (locationId == LocationId.Loc_01)
            {
                DecreaseDifficulty(ref mobDifficulty, 1);
            }
            else if (locationId >= LocationId.Loc_05 && locationId <= LocationId.Loc_07)
            {
                var chanceToIncreaseDifficulty = 50;
                if (Randomizer.TryPercentage(chanceToIncreaseDifficulty))
                {
                    IncreaseDifficulty(ref mobDifficulty, 1);
                }
            }

            byte increasePercents = mobDifficulty switch
            {
                MobDifficulty.END_GAME => 15,
                MobDifficulty.END_GAME_PLUS => 30,
                _ => 0
            };

            var levelRange = mobDifficulty.GetMobLevelRange();
            var mobLevel = new Random().Next(levelRange.minLevel, levelRange.maxLevel + 1);
            var visualLevel = mobDifficulty < MobDifficulty.END_GAME ? mobLevel : 35;
            return new MobDataBuilder(mobLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(levelRange.minLevel, levelRange.maxLevel)
                .IncreaseResistanceByPercents(increasePercents)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(levelRange.minLevel, levelRange.maxLevel)
                .IncreaseDamageValuesByPercents(increasePercents)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(locationId, excludeNames)
                .SetVisualLevel(visualLevel)
                .GetResult();
        }

        public CrossroadsMobData GenerateMobForCrossroads(MobDifficulty mobDifficulty, int crossId, List<string> excludeNames, List<ResourceId> excludeFruits)
        {
            byte increasePercents = mobDifficulty switch
            {
                MobDifficulty.END_GAME => 15,
                MobDifficulty.END_GAME_PLUS => 30,
                _ => 0
            };

            var additionalVisualLevels = 0;
            if (crossId >= 4)
            {
                var chanceToIncreaseDifficulty = 25;
                if (mobDifficulty < MobDifficulty.END_GAME_PLUS && Randomizer.TryPercentage(chanceToIncreaseDifficulty))
                {                    
                    IncreaseDifficulty(ref mobDifficulty, 1);
                    additionalVisualLevels++;
                }
                if (crossId >= 6)
                {
                    var chanceToIncreasePower = 35;
                    if (Randomizer.TryPercentage(chanceToIncreasePower))
                    {
                        increasePercents += 15;
                        additionalVisualLevels++;
                    }
                }
            }

            var index = new Random().Next(crossroadLocationNames.Length);
            var locationForName = crossroadLocationNames[index];
            var levelRange = mobDifficulty.GetMobLevelRange();
            var mobLevel = new Random().Next(levelRange.minLevel, levelRange.maxLevel + 1);
            var visualLevel = mobDifficulty < MobDifficulty.END_GAME ? mobLevel : 35;
            visualLevel += additionalVisualLevels;
            var mobData = new MobDataBuilder(mobLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(levelRange.minLevel, levelRange.maxLevel)
                .IncreaseResistanceByPercents(increasePercents)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(levelRange.minLevel, levelRange.maxLevel)
                .IncreaseDamageValuesByPercents(increasePercents)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(locationForName, excludeNames)
                .SetVisualLevel(visualLevel)
                .GetResult();

            var crossroadsMobData = (CrossroadsMobData)mobData;
            do
            {
                var fruitTypeIndex = new Random().Next(crossroadFruits.Length);
                crossroadsMobData.fruitId = crossroadFruits[fruitTypeIndex];
            } while (excludeFruits.Contains(crossroadsMobData.fruitId));

            return crossroadsMobData;
        }



        private void DecreaseDifficulty(ref MobDifficulty mobDifficulty, int grades)
        {
            var difficulty = (int)mobDifficulty;
            difficulty -= grades;
            mobDifficulty = difficulty < 0 ? MobDifficulty.HALL_3_START : (MobDifficulty)difficulty;
        }

        private void IncreaseDifficulty(ref MobDifficulty mobDifficulty, int grades)
        {
            var difficulty = (int)mobDifficulty;
            difficulty += grades;
            mobDifficulty = difficulty > (int)MobDifficulty.END_GAME_PLUS ? MobDifficulty.END_GAME_PLUS : (MobDifficulty)difficulty;
        }

    }
}
