using Avalonia.Controls;
using ReactiveUI;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Views.Editor.QuestsEditor;

namespace GameDataEditor.ViewModels.Editor.QuestsEditor;

public class StageInspectorViewModel : ViewModelBase
{
    private QuestStage? _stage;
    private bool _hasJumpToStageIfNewSession;
    private int? _jumpToStageIfNewSessionCache;
    private UserControl? _specialStageInspector;

    public QuestStage? stage
    {
        get => _stage;
        set => this.RaiseAndSetIfChanged(ref _stage, value);
    }

    public bool hasJumpToStageIfNewSession
    {
        get => _hasJumpToStageIfNewSession;
        set
        {
            this.RaiseAndSetIfChanged(ref _hasJumpToStageIfNewSession, value);
            if (value && _jumpToStageIfNewSessionCache.HasValue)
            {
                _stage.jumpToStageIfNewSession = _jumpToStageIfNewSessionCache;
            }
            else
            {
                _jumpToStageIfNewSessionCache = _stage.jumpToStageIfNewSession;
                _stage.jumpToStageIfNewSession = null;
            }
        }
    }

    public UserControl? specialStageInspector
    {
        get => _specialStageInspector;
        set => this.RaiseAndSetIfChanged(ref _specialStageInspector, value);
    }

    public StageInspectorViewModel()
    {
    }

    public void ShowStage(QuestStage? stage)
    {
        if (stage == null)
            return;

        this.stage = stage;
        _jumpToStageIfNewSessionCache = stage.jumpToStageIfNewSession;
        hasJumpToStageIfNewSession = stage.jumpToStageIfNewSession.HasValue;

        switch (stage)
        {
            case QuestStageWithTrigger withTrigger:
                specialStageInspector = new StageWithTriggerView();
                specialStageInspector.DataContext = new StageWithTriggerViewModel(withTrigger);
                break;
            case QuestStageWithDefaultReplica withDefaultReplica:
                specialStageInspector = new StageWithDefaultReplicaView();
                specialStageInspector.DataContext = new StageWithDefaultReplicaViewModel(withDefaultReplica);
                break;
            case QuestStageWithReplica withReplica:
                specialStageInspector = new StageWithReplicaView();
                specialStageInspector.DataContext = new StageWithReplicaViewModel(withReplica);
                break;
            case QuestStageWithBattle withBattle:
                specialStageInspector = new StageWithBattleView();
                specialStageInspector.DataContext = new StageWithBattleViewModel(withBattle);
                break;
            case QuestStageWithBattlePoint withBattlePoint:
                specialStageInspector = new StageWithBattlePointView();
                specialStageInspector.DataContext = new StageWithBattlePointViewModel(withBattlePoint);
                break;
        }
    }

}
