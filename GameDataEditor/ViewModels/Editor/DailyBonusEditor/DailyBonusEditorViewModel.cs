using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using MarkOne.Scripts.GameCore.DailyBonus;
using GameDataEditor.ViewModels.Editor.Rewards;

namespace GameDataEditor.ViewModels.Editor.DailyBonusEditor;
public class DailyBonusEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private byte? _selectedDay;
    private DailyBonusData? _dailyBonusData;

    public ObservableCollection<byte> daysList { get; } = new();
    public byte? selectedDay
    {
        get => _selectedDay;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedDay, value);
            if (value != null)
            {
                SetModel(value.Value);
            }
        }
    }
    public EditorListView dailyRewardsListView { get; set; } = new() { DataContext = new EditorRewardsListViewModel() };
    public DailyBonusData? dailyBonusData
    {
        get => _dailyBonusData;
        set => this.RaiseAndSetIfChanged(ref _dailyBonusData, value);
    }

    public ReactiveCommand<Unit, Unit> addDayCommand { get; }
    public ReactiveCommand<Unit, Unit> removeDayCommand { get; }

    public DailyBonusEditorViewModel()
    {
        RefreshDaysList();
        addDayCommand = ReactiveCommand.Create(AddNewTownhall);
        removeDayCommand = ReactiveCommand.Create(RemoveSelectedTownhall);
    }

    private void RefreshDaysList()
    {
        daysList.Clear();
        foreach (var dailyBonusData in gameDataHolder.dailyBonuses.GetAllData())
        {
            daysList.Add(dailyBonusData.id);
        }
    }

    private void SetModel(byte day)
    {
        dailyBonusData = gameDataHolder.dailyBonuses[day];
        if (dailyRewardsListView.DataContext is EditorRewardsListViewModel rewardsListViewModel)
        {
            rewardsListViewModel.SetModel(dailyBonusData.rewards);
        }
    }

    private void AddNewTownhall()
    {
        var maxDay = daysList.Count > 0 ? daysList.Max() : 0;
        var newDay = (byte)(maxDay + 1);
        gameDataHolder.dailyBonuses.AddData(newDay, new DailyBonusData(newDay));
        RefreshDaysList();
        selectedDay = newDay;
    }

    private void RemoveSelectedTownhall()
    {
        if (selectedDay is null)
            return;

        gameDataHolder.dailyBonuses.RemoveData(selectedDay.Value);
        RefreshDaysList();
        selectedDay = null;
    }

}
