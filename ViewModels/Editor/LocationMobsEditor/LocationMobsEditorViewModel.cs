using ReactiveUI;
using System.Collections.ObjectModel;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Views.Editor.LocationMobsEditor;

namespace TextGameRPG.ViewModels.Editor.LocationMobsEditor
{
    internal class LocationMobsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<LocationId>? _selectedLocation;

        public ObservableCollection<EnumValueModel<LocationId>> locations { get; }

        public LocationMobsInspectorView locationInspector { get; }
        public LocationMobsInspectorViewModel locationInspectorViewModel { get; }

        public EnumValueModel<LocationId>? selectedLocation
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
            locations = EnumValueModel<LocationId>.CreateCollection(excludeValue: LocationId.None);
            locationInspector = new LocationMobsInspectorView();
            locationInspector.DataContext = locationInspectorViewModel = new LocationMobsInspectorViewModel();
        }
    }
}
