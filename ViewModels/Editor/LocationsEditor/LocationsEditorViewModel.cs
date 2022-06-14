using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Locations;

namespace TextGameRPG.ViewModels.Editor.LocationsEditor
{
    internal class LocationsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<LocationType>? _selectedLocation;

        public ObservableCollection<EnumValueModel<LocationType>> locations { get; }

        public EnumValueModel<LocationType>? selectedLocation
        {
            get => _selectedLocation;
            set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
        }

        public LocationsEditorViewModel()
        {
            locations = EnumValueModel<LocationType>.CreateCollection(excludeValue: LocationType.None);
        }


    }
}
