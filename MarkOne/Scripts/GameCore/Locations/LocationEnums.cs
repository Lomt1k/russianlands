using FastTelegramBot.DataTypes.InputFiles;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Locations;

public enum LocationId : byte
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
    private static readonly GameDataHolder gameDataHolder = Services.ServiceLocator.Get<GameDataHolder>();

    public static string GetLocalization(this LocationId locationId, GameSession session)
    {
        return Localization.Get(session, $"menu_item_location_{(int)locationId}");
    }

    public static QuestId? GetQuest(this LocationId locationId)
    {
        switch (locationId)
        {
            case LocationId.Loc_01: return QuestId.Loc_01;
            case LocationId.Loc_02: return QuestId.Loc_02;
            case LocationId.Loc_03: return QuestId.Loc_03;
            case LocationId.Loc_04: return QuestId.Loc_04;
            case LocationId.Loc_05: return QuestId.Loc_05;
            case LocationId.Loc_06: return QuestId.Loc_06;
            case LocationId.Loc_07: return QuestId.Loc_07;
        }
        return null;
    }

    public static bool IsLocked(this LocationId locationId, GameSession session)
    {
        var questId = locationId.GetQuest();
        if (questId == null)
            return false;

        var quest = gameDataHolder.quests[questId.Value];
        var isLocked = !quest.IsStarted(session);
        return isLocked;
    }

    public static InputFile? GetPhoto(this LocationId locationId, GameSession session)
    {
        var imageKey = locationId switch
        {
            LocationId.Loc_01 => $"photo_fileId_loc_01",
            LocationId.Loc_02 => $"photo_fileId_loc_02",
            LocationId.Loc_03 => $"photo_fileId_loc_03",
            LocationId.Loc_04 => $"photo_fileId_loc_04",
            LocationId.Loc_05 => $"photo_fileId_loc_05",
            LocationId.Loc_06 => $"photo_fileId_loc_06",
            LocationId.Loc_07 => $"photo_fileId_loc_07",
            _ => string.Empty
        };
        return imageKey is not null
            ? InputFile.FromFileId(Localization.Get(session, imageKey))
            : null;
    }

}
