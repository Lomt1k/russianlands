using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Units.Mobs;

namespace TextGameRPG.ViewModels.Editor.LocationMobsEditor
{
    internal class LocationMobsInspectorViewModel : ViewModelBase
    {
        private static readonly GameDataHolder gameDataHolder = Services.Get<GameDataHolder>();

        private LocationMobData? _locationMobData;
        private byte? _selectedTownHall;

        public ObservableCollection<byte> townHallsList { get; } = new();

        public LocationMobData? locationMobData
        {
            get => _locationMobData;
            set => this.RaiseAndSetIfChanged(ref _locationMobData, value);
        }
        public byte? selectedTownHall
        {
            get => _selectedTownHall;
            set => this.RaiseAndSetIfChanged(ref _selectedTownHall, value);
        }
        

        public void Show(LocationType locationType)
        {
            var locationMobs = gameDataHolder.locationGeneratedMobs;
            if (!locationMobs.ContainsKey(locationType))
            {
                locationMobs.AddData(locationType, new LocationMobData() { id = locationType });
            }
            locationMobData = locationMobs[locationType];
            townHallsList.Clear();
            townHallsList.AddRange(locationMobData.dataByTownhall.Keys);
        }
    }
}
