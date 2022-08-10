﻿using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceIron : IResource
    {
        public ResourceType resourceType => ResourceType.Iron;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceIron;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceIron = value;
        }
    }
}
