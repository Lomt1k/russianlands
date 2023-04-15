﻿using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Scripts.GameCore.Buildings.Data;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameDataBase;
using TextGameRPG.Views.UserControls;

namespace TextGameRPG.ViewModels.Editor.BuildingsEditor
{
    public class BuildingInspectorViewModel : ViewModelBase
    {
        private static readonly GameDataBase gameDataBase = Services.Get<GameDataBase>();

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
            if (!gameDataBase.buildings.ContainsKey(id))
            {
                var newData = new BuildingData()
                {
                    id = id
                };
                gameDataBase.buildings.AddData(id, newData);
                gameDataBase.buildings.Save();
            }
            tempBuilding = gameDataBase.buildings[id].Clone();

            UserControlsHelper.RefillObjectEditorsCollection(levelViews, tempBuilding.levels);
        }

        private void AddNewLevel()
        {
            if (_tempBuilding == null)
                return;

            SaveChanges();

            var newLevel = _tempBuilding.buildingType.CreateNewLevelInfo();
            _tempBuilding.levels.Add(newLevel);
            UserControlsHelper.RefillObjectEditorsCollection(levelViews, _tempBuilding.levels);
        }

        private void RemoveSelectedLevel()
        {
            if (_tempBuilding == null || _selectedLevelView == null)
                return;

            SaveChanges();

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
            gameDataBase.buildings.ChangeData(_tempBuilding.id, _tempBuilding);
        }

        private void ResetChanges()
        {
            var locationType = (BuildingType)_tempBuilding.id;
            Show(locationType);
        }

    }
}
