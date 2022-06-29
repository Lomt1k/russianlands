﻿
namespace TextGameRPG.Scripts.GameCore.Locations
{
    public class Location
    {
        public LocationType type { get; }
        public LocationData data { get; }

        public Location(LocationType _type)
        {
            type = _type;
            data = GameDataBase.GameDataBase.instance.locations[(int)type];
        }

    }

}
