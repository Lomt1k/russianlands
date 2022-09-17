using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceWood : IResource
    {
        public ResourceType resourceType => ResourceType.Wood;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceWood;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceWood = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceWood += value;
        }

        public bool IsUnlocked(GameSession session)
        {
            return true;
        }

        public int GetResourceLimit(GameSession session)
        {
            var buildingData = session.profile.buildingsData;
            var storage = (StorageBuildingBase)BuildingType.WoodStorage.GetBuilding();
            return storage.GetCurrentLevelResourceLimit(buildingData);
        }

    }
}
