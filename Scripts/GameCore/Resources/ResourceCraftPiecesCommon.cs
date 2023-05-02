﻿using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources;

public class ResourceCraftPiecesCommon : IResource
{
    public ResourceId resourceId => ResourceId.CraftPiecesCommon;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceCraftPiecesCommon;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesCommon = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceCraftPiecesCommon += value;
    }

    public bool IsUnlocked(GameSession session)
    {
        // ignored
        return true;
    }

    public int GetResourceLimit(GameSession session)
    {
        return int.MaxValue;
    }

}
