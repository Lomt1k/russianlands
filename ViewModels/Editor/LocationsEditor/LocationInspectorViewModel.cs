using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.ViewModels.Editor.LocationsEditor
{
    internal class LocationInspectorViewModel : ViewModelBase
    {
        private static DataDictionaryWithIntegerID<LocationData> locationDB => GameDataBase.instance.locations;

        private LocationData? _data;

        public LocationData? data
        {
            get => _data;
            set => this.RaiseAndSetIfChanged(ref _data, value);
        }

        public void Show(LocationType location)
        {
            var id = (int)location;
            if (!locationDB.ContainsKey(id))
            {
                locationDB.AddData(id, new LocationData());
                locationDB.Save();
            }
            data = locationDB[id];
        }

    }
}
