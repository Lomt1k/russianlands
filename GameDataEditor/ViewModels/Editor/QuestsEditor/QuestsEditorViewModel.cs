using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using GameDataEditor.Models;
using GameDataEditor.Models.RegularDialogs;
using MarkOne.Scripts.GameCore.Quests;
using MarkOne.Scripts.GameCore.Quests.QuestStages;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using GameDataEditor.Views.Editor.QuestsEditor;

namespace GameDataEditor.ViewModels.Editor.QuestsEditor;

public class QuestsEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private EnumValueModel<QuestId>? _selectedQuest;
    private QuestData? _quest;
    private QuestStage? _selectedStage;

    public ObservableCollection<EnumValueModel<QuestId>> quests { get; }
    public ObservableCollection<QuestStage> questStages { get; } = new ObservableCollection<QuestStage>();

    public EnumValueModel<QuestId>? selectedQuest
    {
        get => _selectedQuest;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedQuest, value);
            if (value != null)
            {
                _quest = gameDataHolder.quests[value.value];
                selectedStage = null;
                questStages.Clear();
                foreach (var stage in _quest.stages)
                {
                    questStages.Add(stage);
                }
            }
        }
    }
    public QuestStage? selectedStage
    {
        get => _selectedStage;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedStage, value);
            stageInspectorVM.ShowStage(value);
        }
    }

    public StageInspectorView stageInspector { get; }
    public StageInspectorViewModel stageInspectorVM { get; }

    public ReactiveCommand<Unit, Unit> addStageCommand { get; }
    public ReactiveCommand<Unit, Unit> removeStageCommand { get; }

    public QuestsEditorViewModel()
    {
        quests = EnumValueModel<QuestId>.CreateCollection(excludeValue: QuestId.None);
        addStageCommand = ReactiveCommand.Create(AddNewStage);
        removeStageCommand = ReactiveCommand.Create(RemoveSelectedStage);

        stageInspector = new StageInspectorView();
        stageInspector.DataContext = stageInspectorVM = new StageInspectorViewModel();
    }

    public void AddNewStage()
    {
        RegularDialogHelper.ShowItemSelectionDialog("Select stage type:", new Dictionary<string, Action>()
        {
            { "Stage With Trigger", AddNewStageWithEndingTrigger },
            { "Stage With Default Replica", AddNewStageWithDefaultReplica },
            { "Stage With Replica", AddNewStageWithReplica },
            { "Stage With Battle", AddNewStageWithBattle },
            { "Stage With Battle Point", AddNewStageWithBattlePoint },
        });
    }

    private void AddNewStageWithDefaultReplica()
    {
        var stage = new QuestStageWithDefaultReplica()
        {
            id = GetDefaultIdForNewStage(),
            comment = "New Default Replica"
        };
        _quest.stages.Add(stage);
        _quest.stages = _quest.stages.OrderBy(x => x.id).ToList();
        questStages.Add(stage);
    }

    private void AddNewStageWithReplica()
    {
        var stage = new QuestStageWithReplica()
        {
            id = GetDefaultIdForNewStage(),
            comment = "New Replica"
        };
        _quest.stages.Add(stage);
        _quest.stages = _quest.stages.OrderBy(x => x.id).ToList();
        questStages.Add(stage);
    }

    private void AddNewStageWithEndingTrigger()
    {
        var stage = new QuestStageWithTrigger()
        {
            id = GetDefaultIdForNewStage()
        };
        _quest.stages.Add(stage);
        _quest.stages = _quest.stages.OrderBy(x => x.id).ToList();
        questStages.Add(stage);
    }

    private void AddNewStageWithBattle()
    {
        var stage = new QuestStageWithBattle()
        {
            id = GetDefaultIdForNewStage()
        };
        _quest.stages.Add(stage);
        _quest.stages = _quest.stages.OrderBy(x => x.id).ToList();
        questStages.Add(stage);
    }

    private void AddNewStageWithBattlePoint()
    {
        var stage = new QuestStageWithBattlePoint()
        {
            id = GetDefaultIdForNewStage()
        };
        _quest.stages.Add(stage);
        _quest.stages = _quest.stages.OrderBy(x => x.id).ToList();
        questStages.Add(stage);
    }

    private int GetDefaultIdForNewStage()
    {
        if (questStages.Count == 0)
            return 100;

        var maxId = questStages.Max(x => x.id);
        return (maxId / 100 * 100) + 100;
    }

    public void RemoveSelectedStage()
    {
        if (selectedStage == null)
            return;

        _quest.stages.Remove(selectedStage);
        questStages.Remove(selectedStage);
        selectedStage = null;
    }


}
