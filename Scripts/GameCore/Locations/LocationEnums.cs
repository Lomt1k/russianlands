using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Locations
{
    public enum LocationType : byte
    {
        None = 0,
        Loc_01 = 1,
        Loc_02 = 2,
        Loc_03 = 3,
        Loc_04 = 4,
        Loc_05 = 5,
        Loc_06 = 6,
        Loc_07 = 7,
    }

    public static class LocationExtensions
    {
        public static string GetLocalization(this LocationType locationType, GameSession session)
        {
            return Localization.Get(session, $"menu_item_location_{(int)locationType}");
        }

        public static QuestType? GetQuest(this LocationType locationType)
        {
            switch (locationType)
            {
                case LocationType.Loc_01: return QuestType.Loc_01;
                case LocationType.Loc_02: return QuestType.Loc_02;
                case LocationType.Loc_03: return QuestType.Loc_03;
                case LocationType.Loc_04: return QuestType.Loc_04;
                case LocationType.Loc_05: return QuestType.Loc_05;
                case LocationType.Loc_06: return QuestType.Loc_06;
                case LocationType.Loc_07: return QuestType.Loc_07;
            }
            return null;
        }

        public static bool IsLocked(this LocationType locationType, GameSession session)
        {
            var questType = locationType.GetQuest();
            if (questType == null)
                return false;

            var quest = QuestsHolder.GetQuest(questType.Value);
            bool isLocked = !quest.IsStarted(session);
            return isLocked;
        }

    }

}
