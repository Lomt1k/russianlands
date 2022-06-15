using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ReactiveUI;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.ViewModels.Editor.LocationsEditor
{
    internal class LocationInspectorViewModel : ViewModelBase
    {
        private static DataDictionaryWithIntegerID<LocationData> locationDB => GameDataBase.instance.locations;

        private LocationData? _data;
        private ObservableCollection<FieldModel> _generalData = new ObservableCollection<FieldModel>();
        private ObservableCollection<FieldModel> _itemGenerationSettings = new ObservableCollection<FieldModel>();

        public ObservableCollection<FieldModel> generalData => _generalData;
        public ObservableCollection<FieldModel> itemGenerationSettings => _itemGenerationSettings;

        public void Show(LocationType location)
        {
            var id = (int)location;
            if (!locationDB.ContainsKey(id))
            {
                var newData = new LocationData()
                {
                    id = id
                };
                locationDB.AddData(id, newData);
                locationDB.Save();
            }
            _data = locationDB[id];

            _generalData.Clear();
            _itemGenerationSettings.Clear();
            FieldModel.FillObservableCollection(ref _generalData, ref _data);
            FieldModel.FillObservableCollection(ref _itemGenerationSettings, ref _data.itemGenerationSettings);
        }

    }
}
