using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using GameDataEditor.Views.UserControls;
using GameDataEditor.ViewModels.Editor.Rewards;

namespace GameDataEditor.ViewModels.Editor.QuestsEditor;

public class StageWithBattleViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = ServiceLocator.Get<GameDataHolder>();


    private readonly Dictionary<string, int> _mobIds = new Dictionary<string, int>();
    private string? _selectedMob;
    private ObjectPropertiesEditorView? _selectedRewardView;

    public QuestStageWithBattle stage { get; }
    public ObservableCollection<string> mobsList { get; }
    public EditorListView rewardViews { get; }

    public string? selectedMob
    {
        get => _selectedMob;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMob, value);
            if (value != null && _mobIds.ContainsKey(value))
            {
                stage.mobId = _mobIds[value];
            }
        }
    }
    public ObjectPropertiesEditorView? selectedRewardView
    {
        get => _selectedRewardView;
        set => this.RaiseAndSetIfChanged(ref _selectedRewardView, value);
    }

    public StageWithBattleViewModel(QuestStageWithBattle stage)
    {
        this.stage = stage;

        mobsList = new ObservableCollection<string>();
        foreach (var mob in gameDataBase.mobs.GetAllData())
        {
            var mobStr = $"{mob.id} | {mob.debugName}";
            _mobIds[mobStr] = mob.id;
            mobsList.Add(mobStr);

            if (mob.id == stage.mobId)
            {
                selectedMob = mobStr;
            }
        }

        var rewardsViewModel = new EditorRewardsListViewModel();
        rewardsViewModel.SetModel(stage.rewards);
        rewardViews = new EditorListView() { DataContext = rewardsViewModel };
    }

}
