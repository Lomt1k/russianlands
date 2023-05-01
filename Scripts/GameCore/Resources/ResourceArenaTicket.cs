﻿using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    internal class ResourceArenaTicket : IResource
    {
        public ResourceId resourceId => ResourceId.ArenaTicket;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceArenaTicket;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceArenaTicket = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceArenaTicket += value;
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
}
