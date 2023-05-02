using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using MarkOne.Models.UserControls;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Scripts.GameCore.Buildings.Data;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Views.UserControls;

namespace MarkOne.ViewModels.Editor.BuildingsEditor;

public class BuildingInspectorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();

    private BuildingData? _buildingData;
    private ObjectPropertiesEditorView? _selectedLevelView;

    public BuildingData? buildingData
    {
        get => _buildingData;
        set => this.RaiseAndSetIfChanged(ref _buildingData, value);
    }
    public ObservableCollection<ObjectPropertiesEditorView> levelViews { get; }
    public ObjectPropertiesEditorView? selectedLevelView
    {
        get => _selectedLevelView;
        set => this.RaiseAndSetIfChanged(ref _selectedLevelView, value);
    }
    public ReactiveCommand<Unit, Unit> addLevelCommand { get; }
    public ReactiveCommand<Unit, Unit> removeLevelCommand { get; }

    public BuildingInspectorViewModel()
    {
        levelViews = new ObservableCollection<ObjectPropertiesEditorView>();
        addLevelCommand = ReactiveCommand.Create(AddNewLevel);
        removeLevelCommand = ReactiveCommand.Create(RemoveSelectedLevel);
    }

    public void Show(BuildingId buildingId)
    {
        if (!gameDataBase.buildings.ContainsKey(buildingId))
        {
            var newData = new BuildingData()
            {
                id = buildingId
            };
            gameDataBase.buildings.AddData(buildingId, newData);
        }
        buildingData = gameDataBase.buildings[buildingId];

        UserControlsHelper.RefillObjectEditorsCollection(levelViews, buildingData.levels);
    }

    private void AddNewLevel()
    {
        if (_buildingData == null)
            return;

        var newLevel = _buildingData.id.CreateNewLevelInfo();
        _buildingData.levels.Add(newLevel);
        UserControlsHelper.RefillObjectEditorsCollection(levelViews, _buildingData.levels);
    }

    private void RemoveSelectedLevel()
    {
        if (_buildingData == null || _selectedLevelView == null)
            return;

        var buildingLevel = _selectedLevelView.vm.GetEditableObject<BuildingLevelInfo>();
        _buildingData.levels.Remove(buildingLevel);
        UserControlsHelper.RefillObjectEditorsCollection(levelViews, _buildingData.levels);
    }

}
