using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TextGameRPG.Models;
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
