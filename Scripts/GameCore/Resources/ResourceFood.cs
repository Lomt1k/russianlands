﻿using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceFood : IResource
    {
        public ResourceType resourceType => ResourceType.Food;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFood;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFood = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceFood += value;
        }

        public bool IsUnlocked(GameSession session)
        {
            return true;
        }

        public int GetResourceLimit(GameSession session)
        {
            var buildingData = session.profile.buildingsData;
            var storage = (StorageBuildingBase)BuildingType.FoodStorage.GetBuilding();
            return storage.GetCurrentLevelResourceLimit(buildingData);
        }

    }
}
