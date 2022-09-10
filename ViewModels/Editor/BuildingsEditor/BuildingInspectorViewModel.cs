using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Views.UserControls;

namespace TextGameRPG.ViewModels.Editor.BuildingsEditor
{
    public class BuildingInspectorViewModel : ViewModelBase
    {
        private static DataDictionaryWithIntegerID<BuildingData> buildingsDB => GameDataBase.instance.buildings;

        private BuildingData? _tempBuilding;
        private ObjectFieldsEditorView? _selectedLevelView;

        public BuildingData? tempBuilding
        {
            get => _tempBuilding;
            set => this.RaiseAndSetIfChanged(ref _tempBuilding, value);
        }
        public ObservableCollection<ObjectFieldsEditorView> levelViews { get; }
        public ObjectFieldsEditorView? selectedLevelView
        {
            get => _selectedLevelView;
            set => this.RaiseAndSetIfChanged(ref _selectedLevelView, value);
        }
        public ReactiveCommand<Unit, Unit> addLevelCommand { get; }
        public ReactiveCommand<Unit, Unit> removeLevelCommand { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public BuildingInspectorViewModel()
        {
            levelViews = new ObservableCollection<ObjectFieldsEditorView>();
            addLevelCommand = ReactiveCommand.Create(AddNewLevel);
            removeLevelCommand = ReactiveCommand.Create(RemoveSelectedLevel);
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
            tempBuilding = buildingsDB[id].Clone();

            UserControlsHelper.RefillObjectEditorsCollection(levelViews, tempBuilding.levels);
        }

        private void AddNewLevel()
        {
            if (_tempBuilding == null)
                return;

            var newLevel = _tempBuilding.buildingType.CreateNewLevelInfo();
            _tempBuilding.levels.Add(newLevel);
            UserControlsHelper.RefillObjectEditorsCollection(levelViews, _tempBuilding.levels);
        }

        private void RemoveSelectedLevel()
        {
            if (_tempBuilding == null || _selectedLevelView == null)
                return;

            var buildingLevel = _selectedLevelView.vm.GetEditableObject<BuildingLevelInfo>();
            _tempBuilding.levels.Remove(buildingLevel);
            UserControlsHelper.RefillObjectEditorsCollection(levelViews, _tempBuilding.levels);
        }

        private void SaveChanges()
        {
            foreach (var levelView in levelViews)
            {
                levelView.vm.SaveObjectChanges();
            }
            buildingsDB.ChangeData(_tempBuilding.id, _tempBuilding);
        }

        private void ResetChanges()
        {
            var locationType = (BuildingType)_tempBuilding.id;
            Show(locationType);
        }

    }
}
