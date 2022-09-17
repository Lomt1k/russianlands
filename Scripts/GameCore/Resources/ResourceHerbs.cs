using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceHerbs : IResource
    {
        public ResourceType resourceType => ResourceType.Herbs;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceHerbs;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceHerbs = value;
        }

        public void AddValue(ProfileData profileData, int value)
        {
            profileData.resourceHerbs += value;
        }

        public bool IsUnlocked(GameSession session)
        {
            var buildingsData = session.profile.buildingsData;
            var building = BuildingType.TownHall.GetBuilding();
            return building.GetCurrentLevel(buildingsData) >= 4;
        }

        public int GetResourceLimit(GameSession session)
        {
            var buildingData = session.profile.buildingsData;
            var storage = (StorageBuildingBase)BuildingType.HerbsStorage.GetBuilding();
            return storage.GetCurrentLevelResourceLimit(buildingData);
        }

    }
}
