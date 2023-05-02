using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.GameCore.Quests;

public enum QuestId : ushort
{
    None = 0,
    MainQuest = 1,
    Loc_01 = 2,
    Loc_02 = 3,
    Loc_03 = 4,
    Loc_04 = 5,
    Loc_05 = 6,
    Loc_06 = 7,
    Loc_07 = 8,
}

public static class QuestIdEstensions
{
    public static LocationId? GetLocation(this QuestId questId)
    {
        switch (questId)
        {
            case QuestId.Loc_01: return LocationId.Loc_01;
            case QuestId.Loc_02: return LocationId.Loc_02;
            case QuestId.Loc_03: return LocationId.Loc_03;
            case QuestId.Loc_04: return LocationId.Loc_04;
            case QuestId.Loc_05: return LocationId.Loc_05;
            case QuestId.Loc_06: return LocationId.Loc_06;
            case QuestId.Loc_07: return LocationId.Loc_07;
        }
        return null;
    }
}
