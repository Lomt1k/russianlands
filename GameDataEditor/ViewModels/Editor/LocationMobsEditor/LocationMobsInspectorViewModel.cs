﻿using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using MarkOne.Scripts.GameCore.Locations;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units.Mobs;
using GameDataEditor.Views.UserControls;
using GameDataEditor.ViewModels.Editor.Rewards;

namespace GameDataEditor.ViewModels.Editor.LocationMobsEditor;

internal class LocationMobsInspectorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private readonly EditorRewardsListViewModel _battleRewardsViewModel = new();
    private readonly EditorRewardsListViewModel _locationRewardsViewModel = new();

    private LocationMobSettingsData? _locationMobData;
    private byte? _selectedTownHall;
    private LocationMobSettingsDataByTownHall? _townHallData;

    public ObservableCollection<byte> townHallsList { get; } = new();

    public LocationMobSettingsData? locationMobData
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
            if (data != null)
            {
                _battleRewardsViewModel.SetModel(data.battleRewards);
                _locationRewardsViewModel.SetModel(data.locationRewards);
            }
        }
    }
    public LocationMobSettingsDataByTownHall? townHallData
    {
        get => _townHallData;
        set => this.RaiseAndSetIfChanged(ref _townHallData, value);
    }
    public EditorListView battleRewards { get; }
    public EditorListView locationRewards { get; }

    public ReactiveCommand<Unit, Unit> addTownHallCommand { get; }
    public ReactiveCommand<Unit, Unit> removeTownHallCommand { get; }

    public LocationMobsInspectorViewModel()
    {
        battleRewards = new EditorListView() { DataContext = _battleRewardsViewModel };
        locationRewards = new EditorListView() { DataContext = _locationRewardsViewModel };

        addTownHallCommand = ReactiveCommand.Create(AddNewTownHall);
        removeTownHallCommand = ReactiveCommand.Create(RemoveSelectedTownHall);
    }


    public void Show(LocationId locationId)
    {
        var locationMobs = gameDataHolder.locationGeneratedMobs;
        if (!locationMobs.ContainsKey(locationId))
        {
            locationMobs.AddData(locationId, new LocationMobSettingsData() { id = locationId });
        }
        locationMobData = locationMobs[locationId];
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
