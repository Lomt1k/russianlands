using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using ReactiveUI;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.GameDataBase;

namespace TextGameRPG.ViewModels.Editor.BuildingsEditor
{
    public class BuildingInspectorViewModel : ViewModelBase
    {
        private static DataDictionaryWithIntegerID<BuildingData> buildingsDB => GameDataBase.instance.buildings;

        private BuildingData? _tempBuilding;
        private ObservableCollection<FieldModel> _generalData = new ObservableCollection<FieldModel>();
        //private ObservableCollection<FieldModel> _itemGenerationSettings = new ObservableCollection<FieldModel>();

        public ObservableCollection<FieldModel> generalData => _generalData;
        //public ObservableCollection<FieldModel> itemGenerationSettings => _itemGenerationSettings;
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public BuildingInspectorViewModel()
        {
            saveCommand = ReactiveCommand.Create(SaveChanges);
            cancelCommand = ReactiveCommand.Create(ResetChanges);
        }

        public void Show(BuildingType buidlingType)
        {
            var id = (int)buidlingType;
            if (!buildingsDB.ContainsKey(id))
            {
                var newData = new BuildingData()
                {
                    id = id
                };
                buildingsDB.AddData(id, newData);
                buildingsDB.Save();
            }
            _tempBuilding = buildingsDB[id].Clone();

            _generalData.Clear();
            //_itemGenerationSettings.Clear();
            FieldModel.FillObservableCollection(ref _generalData, _tempBuilding);
            //FieldModel.FillObservableCollection(ref _itemGenerationSettings, _tempBuilding.itemGenerationSettings);
        }

        private void SaveChanges()
        {
            UpdateFieldValuesFromModel();
            buildingsDB.ChangeData(_tempBuilding.id, _tempBuilding);
        }

        private void UpdateFieldValuesFromModel()
        {
            var fields = _tempBuilding.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsClass)
                    continue;

                var fieldModel = generalData.Where(x => x.name.Equals(fieldInfo.Name)).First();
                fieldInfo.SetValue(_tempBuilding, fieldModel.value);
            }

            //fields = _tempBuilding.itemGenerationSettings.GetType().GetFields();
            //foreach (var fieldInfo in fields)
            //{
            //    var fieldModel = itemGenerationSettings.Where(x => x.name.Equals(fieldInfo.Name)).First();
            //    fieldInfo.SetValue(_tempBuilding.itemGenerationSettings, fieldModel.value);
            //}
        }

        private void ResetChanges()
        {
            var locationType = (BuildingType)_tempBuilding.id;
            Show(locationType);
        }

    }
}
