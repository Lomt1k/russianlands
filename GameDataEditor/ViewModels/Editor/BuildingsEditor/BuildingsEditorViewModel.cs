using ReactiveUI;
using System.Collections.ObjectModel;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Buildings;
using MarkOne.Views.Editor.BuildingsEditor;

namespace GameDataEditor.ViewModels.Editor.BuildingsEditor;

public class BuildingsEditorViewModel : ViewModelBase
{
    private EnumValueModel<BuildingId>? _selectedBuilding;

    public ObservableCollection<EnumValueModel<BuildingId>> buildings { get; }

    public BuildingInspectorView buildingInspector { get; }
    public BuildingInspectorViewModel buildingInspectorViewModel { get; }

    public EnumValueModel<BuildingId>? selectedBuilding
    {
        get => _selectedBuilding;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBuilding, value);
            if (value != null)
            {
                buildingInspectorViewModel.Show(value.value);
            }
        }
    }

    public BuildingsEditorViewModel()
    {
        buildings = EnumValueModel<BuildingId>.CreateCollection();
        buildingInspector = new BuildingInspectorView();
        buildingInspector.DataContext = buildingInspectorViewModel = new BuildingInspectorViewModel();
    }
}
