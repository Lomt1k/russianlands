using System;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings.General;

public class TownHallBuilding : BuildingBase
{
    public override BuildingId buildingId => BuildingId.TownHall;

    public override byte GetCurrentLevel(ProfileBuildingsData data)
    {
        return data.townHallLevel;
    }

    protected override void SetCurrentLevel(ProfileBuildingsData data, byte level)
    {
        data.townHallLevel = level;
    }

    protected override DateTime GetStartConstructionTime(ProfileBuildingsData data)
    {
        return data.townHallStartConstructionTime;
    }

    protected override void SetStartConstructionTime(ProfileBuildingsData data, DateTime startConstructionTime)
    {
        data.townHallStartConstructionTime = startConstructionTime;
    }

    public override string GetCurrentLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        return Localization.Get(session, "building_TownHall_description");
    }

    public override string GetNextLevelInfo(GameSession session, ProfileBuildingsData data)
    {
        // TODO: Добавить инфу о следующем уровне
        return Localization.Get(session, "building_TownHall_description");
    }

    protected override void OnConstructionEnd(ProfileBuildingsData data, DateTime startConstructionTime, DateTime endConstructionTime)
    {
        var profileData = data.session.profile.data;
        ResetTemporaryArenaShopItems(profileData);
    }

    private void ResetTemporaryArenaShopItems(ProfileData profileData)
    {
        profileData.lastArenaItemsUpdateTime = DateTime.MinValue;
        profileData.arenaItemId_0 = null;
        profileData.arenaItemId_1 = null;
        profileData.arenaItemId_2 = null;
        profileData.arenaItemId_3 = null;
        profileData.arenaItemId_4 = null;
    }

    public override string? GetSpecialConstructionWarning(ProfileBuildingsData data, GameSession session)
    {
        var level = GetCurrentLevel(data);
        var nextLevelInfo = (TownhallLevelInfo)buildingData.levels[level];
        var requiredQuest = nextLevelInfo.requireCompletedQuest;
        if (requiredQuest == QuestId.None || data.session.profile.dynamicData.quests.IsCompleted(requiredQuest))
        {
            return null;
        }
        var locationId = requiredQuest.GetLocation();
        return Localization.Get(session, "building_TownHall_locked", locationId.Value.GetLocalization(data.session));
    }

    public override bool IsNextLevelAvailable(ProfileBuildingsData data)
    {
        var defaultValue = base.IsNextLevelAvailable(data);
        if (!defaultValue)
        {
            return false;
        }

        var level = GetCurrentLevel(data);
        var nextLevelInfo = (TownhallLevelInfo)buildingData.levels[level];
        var requiredQuest = nextLevelInfo.requireCompletedQuest;
        return requiredQuest == QuestId.None || data.session.profile.dynamicData.quests.IsCompleted(requiredQuest);
    }

}
