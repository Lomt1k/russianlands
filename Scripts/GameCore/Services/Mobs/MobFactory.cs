using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Services.Mobs
{
    public class MobFactory : Service
    {
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

        public MobData GenerateMobForLocation(MobDifficulty mobDifficulty, LocationId locationId, List<string>? excludeNames)
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

            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .IncreaseResistanceByPercents(increasePercents)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .IncreaseDamageValuesByPercents(increasePercents)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(locationId, excludeNames)
                .GetResult();
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
