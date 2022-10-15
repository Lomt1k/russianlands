using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.Scripts.GameCore.Quests
{
    public enum QuestType : ushort
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

    public static class QuestTypeEstensions
    {
        public static LocationType? GetQuest(this QuestType questType)
        {
            switch (questType)
            {
                case QuestType.Loc_01: return LocationType.Loc_01;
                case QuestType.Loc_02: return LocationType.Loc_02;
                case QuestType.Loc_03: return LocationType.Loc_03;
                case QuestType.Loc_04: return LocationType.Loc_04;
                case QuestType.Loc_05: return LocationType.Loc_05;
                case QuestType.Loc_06: return LocationType.Loc_06;
                case QuestType.Loc_07: return LocationType.Loc_07;
            }
            return null;
        }
    }

}
