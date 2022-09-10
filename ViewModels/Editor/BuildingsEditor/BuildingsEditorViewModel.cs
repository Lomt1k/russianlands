using System.Collections.ObjectModel;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Buildings;
using TextGameRPG.Views.Editor.BuildingsEditor;

namespace TextGameRPG.ViewModels.Editor.BuildingsEditor
{
    public class BuildingsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<BuildingType>? _selectedBuilding;

        public ObservableCollection<EnumValueModel<BuildingType>> buildings { get; }

        public BuildingInspectorView buildingInspector { get; }
        public BuildingInspectorViewModel buildingInspectorViewModel { get; }

        public EnumValueModel<BuildingType>? selectedBuilding
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
            buildings = EnumValueModel<BuildingType>.CreateCollection();
            buildingInspector = new BuildingInspectorView();
            buildingInspector.DataContext = buildingInspectorViewModel = new BuildingInspectorViewModel();
        }
    }
}
