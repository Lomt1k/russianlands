using MarkOne.Scripts.GameCore.Units;

namespace MarkOne.Scripts.GameCore.Services.Arena;
public static class ArenaHelper
{
    public static ArenaLeague GetActualLeagueForPlayer(Player player)
    {
        var playerLevel = player.level;
        if (playerLevel <= 7)
        {
            return ArenaLeague.HALL_3;
        }
        if (playerLevel <= 10)
        {
            return ArenaLeague.HALL_4;
        }
        var skillLevel = player.skills.GetAverageSkillLevel();
        if (playerLevel <= 15)
        {
            // HALL_5_START: level 11 - 13, skills 0 - 5
            if (playerLevel <= 13 && skillLevel <= 5)
            {
                return ArenaLeague.HALL_5_START;
            }
            // HALL_5_MID: skills 6 - 15
            if (skillLevel <= 15)
            {
                return ArenaLeague.HALL_5_MID;
            }
            // skills 16 - 25
            return ArenaLeague.HALL_5_END;
        }
        if (playerLevel <= 20)
        {
            // HALL_6_START: level 16 - 18, skills 15 - 20
            if (playerLevel <= 18 && skillLevel <= 20)
            {
                return ArenaLeague.HALL_6_START;
            }
            // HALL_6_MID: skills 21 - 35
            if (skillLevel <= 35)
            {
                return ArenaLeague.HALL_6_MID;
            }
            // skills 36 - 50
            return ArenaLeague.HALL_6_END;
        }
        if (playerLevel <= 28)
        {
            // HALL_7_START: level 21 - 24, skills 35 - 45
            if (playerLevel <= 24 && skillLevel <= 45)
            {
                return ArenaLeague.HALL_7_START;
            }
            // HALL_7_MID: skills 46 - 55
            if (skillLevel <= 55)
            {
                return ArenaLeague.HALL_7_MID;
            }
            // skills 56 - 75
            return ArenaLeague.HALL_7_END;
        }
        if (playerLevel <= 35)
        {
            // HALL_8_START: level 29 - 32, skills 55 - 60
            if (playerLevel <= 32 && skillLevel <= 60)
            {
                return ArenaLeague.HALL_8_START;
            }
            // HALL_8_MID: skills 61 - 75
            if (skillLevel <= 75)
            {
                return ArenaLeague.HALL_8_MID;
            }
            // skills 76 - 100
            return ArenaLeague.HALL_8_END;
        }
        return ArenaLeague.HALL_8_END;
    }
}
