using ReactiveUI;
using System.Collections.ObjectModel;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Views.Editor.LocationMobsEditor;

namespace GameDataEditor.ViewModels.Editor.LocationMobsEditor;

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
