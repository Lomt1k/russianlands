using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Views.Editor.LocationsEditor;

namespace TextGameRPG.ViewModels.Editor.LocationsEditor
{
    public class LocationsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<LocationType>? _selectedLocation;

        public ObservableCollection<EnumValueModel<LocationType>> locations { get; }

        public LocationInspectorView locationInspector { get; }
        public LocationInspectorViewModel locationInspectorViewModel { get; }

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

        public LocationsEditorViewModel()
        {
            locations = EnumValueModel<LocationType>.CreateCollection(excludeValue: LocationType.None);
            locationInspector = new LocationInspectorView();
            locationInspector.DataContext = locationInspectorViewModel = new LocationInspectorViewModel();
        }


    }
}
