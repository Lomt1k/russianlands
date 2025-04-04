﻿using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

internal class ResourceFruitCherry : IResource
{
    public ResourceId resourceId => ResourceId.FruitCherry;

    public int GetValue(ProfileData profileData)
    {
        return profileData.resourceFruitCherry;
    }

    public void SetValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitCherry = value;
    }

    public void AddValue(ProfileData profileData, int value)
    {
        profileData.resourceFruitCherry += value;
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
