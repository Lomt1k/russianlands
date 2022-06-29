using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Locations
{
    public static class LocationsHolder
    {
        private static Dictionary<LocationType, Location> _locations;

        static LocationsHolder()
        {
            _locations = new Dictionary<LocationType, Location>();
            var allData = GameDataBase.GameDataBase.instance.locations.GetAllData();
            foreach (var locationData in allData)
            {
                var type = (LocationType)locationData.id;
                var location = new Location(type);
                _locations.Add(type, location);
            }
        }

        public static Location Get(LocationType type)
        {
            return _locations[type];
        }

        public static IEnumerable<Location> GetAll()
        {
            return _locations.Values;
        }

    }
}
