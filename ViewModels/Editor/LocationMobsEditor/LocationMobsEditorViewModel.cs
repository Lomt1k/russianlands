using ReactiveUI;
using System.Collections.ObjectModel;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Views.Editor.LocationMobsEditor;

namespace TextGameRPG.ViewModels.Editor.LocationMobsEditor
{
    internal class LocationMobsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<LocationType>? _selectedLocation;

        public ObservableCollection<EnumValueModel<LocationType>> locations { get; }

        public LocationMobsInspectorView locationInspector { get; }
        public LocationMobsInspectorViewModel locationInspectorViewModel { get; }

        public EnumValueModel<LocationType>? selectedLocation
        {
            get => _selectedLocation;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedLocation, value);
                if (value != null)
                {
                    locationInspectorViewModel.Show(value.value);
                }
            }
        }

        public LocationMobsEditorViewModel()
        {
            locations = EnumValueModel<LocationType>.CreateCollection(excludeValue: LocationType.None);
            locationInspector = new LocationMobsInspectorView();
            locationInspector.DataContext = locationInspectorViewModel = new LocationMobsInspectorViewModel();
        }
    }
}
