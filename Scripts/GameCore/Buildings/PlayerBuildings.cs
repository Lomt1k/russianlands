using System.Collections.Generic;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Buildings
{
    public class PlayerBuildings
    {
        private GameSession _session;
        private ProfileBuildingsData _buildingsData;

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

        // TODO: когда будут готовы все здания - заменить на пробег по enum BuildingType 
        public IEnumerable<BuildingBase> GetAllBuildings()
        {
            yield return BuildingType.TownHall.GetBuilding();
            yield return BuildingType.Tyr.GetBuilding();
            yield return BuildingType.Hospital.GetBuilding();
            yield return BuildingType.AlchemyLab.GetBuilding();
            yield return BuildingType.WeaponsWorkshop.GetBuilding();

            yield return BuildingType.GoldStorage.GetBuilding();
            yield return BuildingType.FoodStorage.GetBuilding();
            yield return BuildingType.HerbsStorage.GetBuilding();
            yield return BuildingType.WoodStorage.GetBuilding();

            yield return BuildingType.GoldProductionFirst.GetBuilding();
            yield return BuildingType.GoldProductionSecond.GetBuilding();
            yield return BuildingType.GoldProductionThird.GetBuilding();
            yield return BuildingType.FoodProductionFirst.GetBuilding();
            yield return BuildingType.FoodProductionSecond.GetBuilding();
            yield return BuildingType.FoodProductionThird.GetBuilding();
            yield return BuildingType.HerbsProductionFirst.GetBuilding();
            yield return BuildingType.HerbsProductionSecond.GetBuilding();
            yield return BuildingType.HerbsProductionThird.GetBuilding();
            yield return BuildingType.WoodProductionFirst.GetBuilding();
            yield return BuildingType.WoodProductionSecond.GetBuilding();

            yield return BuildingType.WarriorTraining.GetBuilding();
            yield return BuildingType.GoldTraining.GetBuilding();
            yield return BuildingType.FoodTraining.GetBuilding();
            yield return BuildingType.HerbsTraining.GetBuilding();
            yield return BuildingType.WoodTraining.GetBuilding();
        }

        public IEnumerable<BuildingBase> GetBuildingsByCategory(BuildingCategory category)
        {
            switch (category)
            {
                case BuildingCategory.General:
                    yield return BuildingType.TownHall.GetBuilding();
                    yield return BuildingType.Tyr.GetBuilding();
                    yield return BuildingType.Hospital.GetBuilding();
                    yield return BuildingType.AlchemyLab.GetBuilding();
                    yield return BuildingType.WeaponsWorkshop.GetBuilding();
                    break;

                case BuildingCategory.Storages:
                    yield return BuildingType.ItemsStorage.GetBuilding();
                    yield return BuildingType.GoldStorage.GetBuilding();
                    yield return BuildingType.FoodStorage.GetBuilding();
                    yield return BuildingType.HerbsStorage.GetBuilding();
                    yield return BuildingType.WoodStorage.GetBuilding();
                    break;

                case BuildingCategory.Production:
                    yield return BuildingType.GoldProductionFirst.GetBuilding();
                    yield return BuildingType.GoldProductionSecond.GetBuilding();
                    yield return BuildingType.GoldProductionThird.GetBuilding();
                    yield return BuildingType.FoodProductionFirst.GetBuilding();
                    yield return BuildingType.FoodProductionSecond.GetBuilding();
                    yield return BuildingType.FoodProductionThird.GetBuilding();
                    yield return BuildingType.HerbsProductionFirst.GetBuilding();
                    yield return BuildingType.HerbsProductionSecond.GetBuilding();
                    yield return BuildingType.HerbsProductionThird.GetBuilding();
                    yield return BuildingType.WoodProductionFirst.GetBuilding();
                    yield return BuildingType.WoodProductionSecond.GetBuilding();
                    break;

                case BuildingCategory.Training:
                    yield return BuildingType.WarriorTraining.GetBuilding();
                    yield return BuildingType.GoldTraining.GetBuilding();
                    yield return BuildingType.FoodTraining.GetBuilding();
                    yield return BuildingType.HerbsTraining.GetBuilding();
                    yield return BuildingType.WoodTraining.GetBuilding();
                    break;
            }
        }

    }
}
