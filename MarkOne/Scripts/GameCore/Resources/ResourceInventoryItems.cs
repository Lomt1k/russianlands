using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

// Псевдо-ресурс, используется для определения размера инвентаря
public class ResourceInventoryItems : IResource
{
    public ResourceId resourceId => ResourceId.InventoryItems;

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
        var itemsStorage = BuildingId.ItemsStorage.GetBuilding();
        var currentLevel = itemsStorage.GetCurrentLevel(buildingsData);
        var storageLevelInfo = (StorageLevelInfo)itemsStorage.buildingData.levels[currentLevel - 1];
        return storageLevelInfo.resourceStorageLimit;
    }

}
