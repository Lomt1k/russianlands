using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using ReactiveUI;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.ViewModels.Editor.LocationsEditor
{
    public class LocationInspectorViewModel : ViewModelBase
    {
        private static DataDictionaryWithIntegerID<LocationData> locationDB => GameDataBase.instance.locations;

        private LocationData? _tempLocation;
        private ObservableCollection<FieldModel> _generalData = new ObservableCollection<FieldModel>();
        private ObservableCollection<FieldModel> _itemGenerationSettings = new ObservableCollection<FieldModel>();

        public ObservableCollection<FieldModel> generalData => _generalData;
        public ObservableCollection<FieldModel> itemGenerationSettings => _itemGenerationSettings;
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public LocationInspectorViewModel()
        {
            saveCommand = ReactiveCommand.Create(SaveChanges);
            cancelCommand = ReactiveCommand.Create(ResetChanges);
        }

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
            _tempLocation = locationDB[id].Clone();

            _generalData.Clear();
            _itemGenerationSettings.Clear();
            FieldModel.FillObservableCollection(ref _generalData, _tempLocation);
            FieldModel.FillObservableCollection(ref _itemGenerationSettings, _tempLocation.itemGenerationSettings);
        }

        private void SaveChanges()
        {
            UpdateFieldValuesFromModel();
            locationDB.ChangeData(_tempLocation.id, _tempLocation);
        }

        private void UpdateFieldValuesFromModel()
        {
            var fields = _tempLocation.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsClass)
                    continue;

                var fieldModel = generalData.Where(x => x.name.Equals(fieldInfo.Name)).First();
                fieldInfo.SetValue(_tempLocation, fieldModel.value);
            }

            fields = _tempLocation.itemGenerationSettings.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                var fieldModel = itemGenerationSettings.Where(x => x.name.Equals(fieldInfo.Name)).First();
                fieldInfo.SetValue(_tempLocation.itemGenerationSettings, fieldModel.value);
            }
        }

        private void ResetChanges()
        {
            var locationType = (LocationType)_tempLocation.id;
            Show(locationType);
        }

    }
}
