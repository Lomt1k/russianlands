using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using TextGameRPG.Models;
using TextGameRPG.Models.RegularDialogs;
using TextGameRPG.Scripts.GameCore.Quests;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using TextGameRPG.Views.Editor.QuestsEditor;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class QuestsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<QuestId>? _selectedQuest;
        private Quest? _quest;
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
                    _quest = QuestsHolder.GetQuest(value.value);
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
                stageInspectorVM.SaveChanges();
                this.RaiseAndSetIfChanged(ref _selectedStage, value);
                stageInspectorVM.ShowStage(value);
            }
        }

        public StageInspectorView stageInspector { get; }
        public StageInspectorViewModel stageInspectorVM { get; }

        public ReactiveCommand<Unit,Unit> addStageCommand { get; }
        public ReactiveCommand<Unit,Unit> removeStageCommand { get; }
        public ReactiveCommand<Unit,Unit> saveQuestChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> resetQuestChangesCommand { get; }

        public QuestsEditorViewModel()
        {
            quests = EnumValueModel<QuestId>.CreateCollection(excludeValue: QuestId.None);
            addStageCommand = ReactiveCommand.Create(AddNewStage);
            removeStageCommand = ReactiveCommand.Create(RemoveSelectedStage);
            saveQuestChangesCommand = ReactiveCommand.Create(SaveQuestChanges);
            resetQuestChangesCommand = ReactiveCommand.Create(ReloadQuestFromData);

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
            questStages.Add(stage);
        }

        private void AddNewStageWithReplica()
        {
            var stage = new QuestStageWithReplica()
            {
                id = GetDefaultIdForNewStage(),
                comment = "New Replica"
            };
            questStages.Add(stage);
        }

        private void AddNewStageWithEndingTrigger()
        {
            var stage = new QuestStageWithTrigger()
            {
                id = GetDefaultIdForNewStage()
            };
            questStages.Add(stage);
        }

        private void AddNewStageWithBattle()
        {
            var stage = new QuestStageWithBattle()
            {
                id = GetDefaultIdForNewStage()
            };
            questStages.Add(stage);
        }

        private void AddNewStageWithBattlePoint()
        {
            var stage = new QuestStageWithBattlePoint()
            {
                id = GetDefaultIdForNewStage()
            };
            questStages.Add(stage);
        }

        private int GetDefaultIdForNewStage()
        {
            if (questStages.Count == 0)
                return 100;

            var maxId = questStages.Max(x => x.id);
            return maxId / 100 * 100 + 100;
        }

        public void RemoveSelectedStage()
        {
            if (selectedStage == null)
                return;

            questStages.Remove(selectedStage);
            selectedStage = null;
        }

        public void SaveQuestChanges()
        {
            if (_quest == null || _selectedQuest == null)
                return;

            stageInspectorVM?.SaveChanges();
            _quest.stages = questStages.OrderBy(x => x.id).ToList();
            QuestsHolder.SaveQuest(_selectedQuest.value);
            ReloadQuestFromData();
        }

        public void ReloadQuestFromData()
        {
            if (selectedQuest == null)
                return;

            QuestsHolder.LoadQuest(selectedQuest.value);
            selectedQuest = selectedQuest;
        }


    }
}
