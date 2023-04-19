using TextGameRPG.Scripts.GameCore.Units;

namespace TextGameRPG.Scripts.GameCore.Services.MobGenerator
{
    public static class MobDifficultyCalculator
    {
        private static int MIN_LEVEL_FOR_HALL_8 = 29;

        public static MobDifficulty GetActualDifficultyForPlayer(Player player)
        {
            var averageSkillLevel = player.skills.GetAverageSkillLevel();
            if (averageSkillLevel >= 95 && player.level >= MIN_LEVEL_FOR_HALL_8)
            {
                return MobDifficulty.END_GAME_PLUS;
            }
            if (averageSkillLevel >= 85 && player.level >= MIN_LEVEL_FOR_HALL_8)
            {
                return MobDifficulty.END_GAME;
            }

            if (player.level >= 34) return MobDifficulty.HALL_8_END;
            if (player.level >= 32) return MobDifficulty.HALL_8_MID;
            if (player.level >= 29) return MobDifficulty.HALL_8_START;
            if (player.level >= 27) return MobDifficulty.HALL_7_END;
            if (player.level >= 24) return MobDifficulty.HALL_7_MID;
            if (player.level >= 21) return MobDifficulty.HALL_7_START;
            if (player.level >= 20) return MobDifficulty.HALL_6_END;
            if (player.level >= 18) return MobDifficulty.HALL_6_MID;
            if (player.level >= 16) return MobDifficulty.HALL_6_START;
            if (player.level >= 14) return MobDifficulty.HALL_5_END;
            if (player.level >= 11) return MobDifficulty.HALL_5_START;
            if (player.level >= 10) return MobDifficulty.HALL_4_END;
            if (player.level >= 8) return MobDifficulty.HALL_4_START;
            if (player.level >= 6) return MobDifficulty.HALL_3_END;

            return MobDifficulty.HALL_3_START;
        }
    }
}
