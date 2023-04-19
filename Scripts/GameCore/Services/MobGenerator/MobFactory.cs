using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Services.MobGenerator;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.Scripts.GameCore.Services
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
                _ => GenerateMobFor_Localtion_01(mobDifficulty)
            };
        }

        private MobData GenerateMobFor_Localtion_01(MobDifficulty mobDifficulty)
        {
            if (mobDifficulty > MobDifficulty.HALL_6_MID)
            {
                DowngradeDifficulty(ref mobDifficulty, 2);
            }
            else if (mobDifficulty > MobDifficulty.HALL_5_START)
            {
                DowngradeDifficulty(ref mobDifficulty, 1);
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



        private void DowngradeDifficulty(ref MobDifficulty mobDifficulty, int grades)
        {
            var difficulty = (int)mobDifficulty;
            difficulty -= grades;
            mobDifficulty = difficulty < 0 ? MobDifficulty.HALL_3_START : (MobDifficulty)difficulty;
        }

        private void UpgradeDifficulty(ref MobDifficulty mobDifficulty, int grades)
        {
            var difficulty = (int)mobDifficulty;
            difficulty += grades;
            mobDifficulty = difficulty > (int)MobDifficulty.END_GAME_PLUS ? MobDifficulty.END_GAME_PLUS : (MobDifficulty)difficulty;
        }

    }
}
