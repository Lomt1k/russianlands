using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
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
        private LocationMobDataByTownHall? _townHallData;

        public ObservableCollection<byte> townHallsList { get; } = new();

        public LocationMobData? locationMobData
        {
            get => _locationMobData;
            set => this.RaiseAndSetIfChanged(ref _locationMobData, value);
        }
        public byte? selectedTownHall
        {
            get => _selectedTownHall;
            set 
            {
                this.RaiseAndSetIfChanged(ref _selectedTownHall, value);
                var data = value.HasValue ? locationMobData.dataByTownhall[value.Value] : null;
                townHallData = data;
            }
        }
        public LocationMobDataByTownHall? townHallData
        {
            get => _townHallData;
            set => this.RaiseAndSetIfChanged(ref _townHallData, value);
        }

        public ReactiveCommand<Unit, Unit> addTownHallCommand { get; }
        public ReactiveCommand<Unit, Unit> removeTownHallCommand { get; }

        public LocationMobsInspectorViewModel()
        {
            addTownHallCommand = ReactiveCommand.Create(AddNewTownHall);
            removeTownHallCommand = ReactiveCommand.Create(RemoveSelectedTownHall);
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

        private void AddNewTownHall()
        {
            var newTownHall = locationMobData.AddNewTownHall();
            Show(locationMobData.id);
            selectedTownHall = newTownHall;
        }

        private void RemoveSelectedTownHall()
        {
            if (selectedTownHall is null)
                return;

            locationMobData.RemoveTownHall(selectedTownHall.Value);
            selectedTownHall = null;
            Show(locationMobData.id);
        }

    }
}
