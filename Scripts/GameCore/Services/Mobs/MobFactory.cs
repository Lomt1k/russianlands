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

        public MobData GenerateMobForLocation(MobDifficulty mobDifficulty, LocationType locationType)
        {
            return locationType switch
            {
                LocationType.Loc_01 => GenerateMobFor_Localtion_01(mobDifficulty),
                LocationType.Loc_02 => GenerateMobFor_Localtion_02(mobDifficulty),
                LocationType.Loc_03 => GenerateMobFor_Localtion_03(mobDifficulty),
                LocationType.Loc_04 => GenerateMobFor_Localtion_04(mobDifficulty),
                LocationType.Loc_05 => GenerateMobFor_Localtion_05(mobDifficulty),
                LocationType.Loc_06 => GenerateMobFor_Localtion_06(mobDifficulty),
                LocationType.Loc_07 => GenerateMobFor_Localtion_07(mobDifficulty),
                _ => GenerateMobFor_Localtion_01(mobDifficulty)
            };
        }

        private MobData GenerateMobFor_Localtion_01(MobDifficulty mobDifficulty)
        {
            DecreaseDifficulty(ref mobDifficulty, 1);

            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_01)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_02(MobDifficulty mobDifficulty)
        {
            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_02)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_03(MobDifficulty mobDifficulty)
        {
            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_03)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_04(MobDifficulty mobDifficulty)
        {
            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_04)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_05(MobDifficulty mobDifficulty)
        {
            var chanceToIncreaseDifficulty = 50;
            if (Randomizer.TryPercentage(chanceToIncreaseDifficulty))
            {
                var grades = Randomizer.TryPercentage(15) ? 2 : 1;
                IncreaseDifficulty(ref mobDifficulty, grades);
            }

            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_05)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_06(MobDifficulty mobDifficulty)
        {
            var chanceToIncreaseDifficulty = 50;
            if (Randomizer.TryPercentage(chanceToIncreaseDifficulty))
            {
                var grades = Randomizer.TryPercentage(25) ? 2 : 1;
                IncreaseDifficulty(ref mobDifficulty, grades);
            }

            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_06)
                .GetResult();
        }

        private MobData GenerateMobFor_Localtion_07(MobDifficulty mobDifficulty)
        {
            var chanceToIncreaseDifficulty = 50;
            if (Randomizer.TryPercentage(chanceToIncreaseDifficulty))
            {
                var grades = Randomizer.TryPercentage(35) ? 2 : 1;
                IncreaseDifficulty(ref mobDifficulty, grades);
            }

            var mobLevels = mobDifficulty.GetMobLevelRange();
            return new MobDataBuilder(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeHealthByPercents(10)
                .CopyResistanceFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeResistanceByPercents(10)
                .ShuffleResistanceValues()
                .CopyAttacksFromQuestMob(mobLevels.minLevel, mobLevels.maxLevel)
                .RandomizeDamageValuesByPercents(10)
                .ShuffleDamageValues()
                .SetRandomName(LocationType.Loc_07)
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
