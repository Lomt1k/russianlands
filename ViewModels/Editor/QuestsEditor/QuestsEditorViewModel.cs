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

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    internal class QuestsEditorViewModel : ViewModelBase
    {
        private EnumValueModel<QuestType>? _selectedQuest;
        private Quest? _quest;
        private QuestStage? _selectedStage;

        public ObservableCollection<EnumValueModel<QuestType>> quests { get; }
        public ObservableCollection<QuestStage> questStages { get; } = new ObservableCollection<QuestStage>();

        public EnumValueModel<QuestType>? selectedQuest
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
                this.RaiseAndSetIfChanged(ref _selectedStage, value);
                if (value != null)
                {
                    //TODO
                }
            }
        }

        public ReactiveCommand<Unit, Unit> addStageCommand { get;  }
        public ReactiveCommand<Unit, Unit> removeStageCommand { get;  }

        public QuestsEditorViewModel()
        {
            quests = EnumValueModel<QuestType>.CreateCollection(excludeValue: QuestType.None);
            addStageCommand = ReactiveCommand.Create(AddNewStage);
            removeStageCommand = ReactiveCommand.Create(RemoveSelectedStage);
        }

        public void AddNewStage()
        {
            RegularDialogHelper.ShowItemSelectionDialog("Select stage type:", new Dictionary<string, Action>()
            {
                { "Default", AddNewStageWithEndingTrigger },
                { "Replica", AddNewStageWithReplica }
            });
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
            var stage = new QuestStageWithEndingTrigger()
            {
                id = GetDefaultIdForNewStage()
            };
            questStages.Add(stage);
        }

        private int GetDefaultIdForNewStage()
        {
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


    }
}
