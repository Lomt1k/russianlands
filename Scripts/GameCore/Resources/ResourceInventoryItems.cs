using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Data;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    // Псевдо-ресурс, используется для определения размера инвентаря
    public class ResourceInventoryItems : IResource
    {
        public ResourceType resourceType => ResourceType.InventoryItems;

        public int GetValue(ProfileData profileData)
        {
            return profileData.session.player.inventory.itemsCount;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            // ignored
        }

        public void AddValue(ProfileData profileData, int value)
        {
            // ignored
        }

        public bool IsUnlocked(GameSession session)
        {
            return true;
        }

        public int GetResourceLimit(GameSession session)
        {
            var buildingsData = session.profile.buildingsData;
            var itemsStorage = BuildingType.ItemsStorage.GetBuilding();
            var currentLevel = itemsStorage.GetCurrentLevel(buildingsData);
            var storageLevelInfo = (StorageLevelInfo)itemsStorage.buildingData.levels[currentLevel  - 1];
            return storageLevelInfo.resourceStorageLimit;
        }
        
    }
}
