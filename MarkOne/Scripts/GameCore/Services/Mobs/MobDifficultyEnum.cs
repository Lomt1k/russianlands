namespace MarkOne.Scripts.GameCore.Services.Mobs;

public enum MobDifficulty : byte
{
    HALL_3_START = 0,
    HALL_3_END,
    HALL_4_START,
    HALL_4_END,
    HALL_5_START,
    HALL_5_END,
    HALL_6_START,
    HALL_6_MID,
    HALL_6_END,
    HALL_7_START,
    HALL_7_MID,
    HALL_7_END,
    HALL_8_START,
    HALL_8_MID,
    HALL_8_END,

    END_GAME,
    END_GAME_PLUS,
}

public static class MobDifficultyExtensions
{
    public static (int minLevel, int maxLevel) GetMobLevelRange(this MobDifficulty mobDifficulty)
    {
        return mobDifficulty switch
        {
            MobDifficulty.HALL_3_START => (3, 5),
            MobDifficulty.HALL_3_END => (5, 6),
            MobDifficulty.HALL_4_START => (7, 8),
            MobDifficulty.HALL_4_END => (9, 10),
            MobDifficulty.HALL_5_START => (11, 12),
            MobDifficulty.HALL_5_END => (12, 13),
            MobDifficulty.HALL_6_START => (15, 16),
            MobDifficulty.HALL_6_MID => (16, 17),
            MobDifficulty.HALL_6_END => (18, 19),
            MobDifficulty.HALL_7_START => (21, 22),
            MobDifficulty.HALL_7_MID => (22, 23),
            MobDifficulty.HALL_7_END => (24, 26),
            MobDifficulty.HALL_8_START => (28, 29),
            MobDifficulty.HALL_8_MID => (30, 32),
            MobDifficulty.HALL_8_END => (33, 35),
            _ => (34, 35)
        };
    }
}
