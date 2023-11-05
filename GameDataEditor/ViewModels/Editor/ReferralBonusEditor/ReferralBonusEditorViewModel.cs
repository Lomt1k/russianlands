using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using GameDataEditor.ViewModels.Editor.Rewards;
using MarkOne.Scripts.GameCore.ReferralSystem;

namespace GameDataEditor.ViewModels.Editor.ReferralBonusEditor;
public class ReferralBonusEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private byte? _selectedBonus;
    private ReferralBonusData? _bonusData;

    public ObservableCollection<byte> bonusList { get; } = new();
    public byte? selectedBonus
    {
        get => _selectedBonus;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBonus, value);
            if (value != null)
            {
                SetModel(value.Value);
            }
        }
    }
    public EditorListView rewardsListView { get; set; } = new() { DataContext = new EditorRewardsListViewModel() };
    public ReferralBonusData? bonusData
    {
        get => _bonusData;
        set => this.RaiseAndSetIfChanged(ref _bonusData, value);
    }

    public ReactiveCommand<Unit, Unit> addBonusCommand { get; }
    public ReactiveCommand<Unit, Unit> removeBonusCommand { get; }

    public ReferralBonusEditorViewModel()
    {
        RefreshDaysList();
        addBonusCommand = ReactiveCommand.Create(AddNewBonus);
        removeBonusCommand = ReactiveCommand.Create(RemoveSelectedBonus);
    }

    private void RefreshDaysList()
    {
        bonusList.Clear();
        foreach (var dailyBonusData in gameDataHolder.referralBonuses.GetAllData())
        {
            bonusList.Add(dailyBonusData.id);
        }
    }

    private void SetModel(byte id)
    {
        bonusData = gameDataHolder.referralBonuses[id];
        if (rewardsListView.DataContext is EditorRewardsListViewModel rewardsListViewModel)
        {
            rewardsListViewModel.SetModel(bonusData.rewards);
        }
    }

    private void AddNewBonus()
    {
        var maxBonusId = bonusList.Count > 0 ? bonusList.Max() : 0;
        var newBonusId = (byte)(maxBonusId + 1);
        gameDataHolder.referralBonuses.AddData(newBonusId, new ReferralBonusData(newBonusId));
        RefreshDaysList();
        selectedBonus = newBonusId;
    }

    private void RemoveSelectedBonus()
    {
        if (selectedBonus is null)
            return;

        gameDataHolder.referralBonuses.RemoveData(selectedBonus.Value);
        RefreshDaysList();
        selectedBonus = null;
    }

}
