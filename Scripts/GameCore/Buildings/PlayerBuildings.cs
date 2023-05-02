using System.Collections.Generic;
using MarkOne.Scripts.Bot.DataBase.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Buildings;

public class PlayerBuildings
{
    private readonly GameSession _session;
    private readonly ProfileBuildingsData _buildingsData;

    public PlayerBuildings(GameSession session)
    {
        _session = session;
        _buildingsData = session.profile.buildingsData;
    }

    public bool HasImportantUpdates()
    {
        var buildings = GetAllBuildings();
        foreach (var building in buildings)
        {
            if (building.HasImportantUpdates(_buildingsData))
            {
                return true;
            }
        }
        return false;
    }

    public IEnumerable<BuildingBase> GetAllBuildings()
    {
        foreach (BuildingId buildingId in System.Enum.GetValues(typeof(BuildingId)))
        {
            yield return buildingId.GetBuilding();
        }
    }

    public IEnumerable<BuildingBase> GetBuildingsByCategory(BuildingCategory category)
    {
        switch (category)
        {
            case BuildingCategory.General:
                yield return BuildingId.TownHall.GetBuilding();
                yield return BuildingId.Tyr.GetBuilding();
                yield return BuildingId.Hospital.GetBuilding();
                yield return BuildingId.AlchemyLab.GetBuilding();
                yield return BuildingId.ElixirWorkshop.GetBuilding();
                yield return BuildingId.WeaponsWorkshop.GetBuilding();
                yield return BuildingId.ArmorWorkshop.GetBuilding();
                yield return BuildingId.Jewerly.GetBuilding();
                yield return BuildingId.ScribesHouse.GetBuilding();
                break;

            case BuildingCategory.Storages:
                yield return BuildingId.ItemsStorage.GetBuilding();
                yield return BuildingId.GoldStorage.GetBuilding();
                yield return BuildingId.FoodStorage.GetBuilding();
                yield return BuildingId.HerbsStorage.GetBuilding();
                yield return BuildingId.WoodStorage.GetBuilding();
                break;

            case BuildingCategory.Production:
                yield return BuildingId.GoldProductionFirst.GetBuilding();
                yield return BuildingId.GoldProductionSecond.GetBuilding();
                yield return BuildingId.GoldProductionThird.GetBuilding();
                yield return BuildingId.FoodProductionFirst.GetBuilding();
                yield return BuildingId.FoodProductionSecond.GetBuilding();
                yield return BuildingId.FoodProductionThird.GetBuilding();
                yield return BuildingId.HerbsProductionFirst.GetBuilding();
                yield return BuildingId.HerbsProductionSecond.GetBuilding();
                yield return BuildingId.HerbsProductionThird.GetBuilding();
                yield return BuildingId.WoodProductionFirst.GetBuilding();
                yield return BuildingId.WoodProductionSecond.GetBuilding();
                break;

            case BuildingCategory.Training:
                yield return BuildingId.WarriorTraining.GetBuilding();
                yield return BuildingId.GoldTraining.GetBuilding();
                yield return BuildingId.FoodTraining.GetBuilding();
                yield return BuildingId.HerbsTraining.GetBuilding();
                yield return BuildingId.WoodTraining.GetBuilding();
                break;
        }
    }

    public bool HasBuilding(BuildingId building)
    {
        return building.GetBuilding().GetCurrentLevel(_buildingsData) > 0;
    }

    public byte GetBuildingLevel(BuildingId building)
    {
        return building.GetBuilding().GetCurrentLevel(_buildingsData);
    }

}
