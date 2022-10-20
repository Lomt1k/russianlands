using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithDefaultReplicaViewModel : ViewModelBase
    {
        private EnumValueModel<ReplicaType> _selectedReplicaType;

        public QuestStageWithDefaultReplica stage { get; }
        public ObservableCollection<EnumValueModel<ReplicaType>> replicaTypes { get; }

        public EnumValueModel<ReplicaType> selectedReplicaType
        {
            get => _selectedReplicaType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedReplicaType, value);
                stage.replicaType = value.value;
            }
        }

        public StageWithDefaultReplicaViewModel(QuestStageWithDefaultReplica stage)
        {
            this.stage = stage;
            replicaTypes = EnumValueModel<ReplicaType>.CreateCollection();
            _selectedReplicaType = replicaTypes.First(x => x.value == stage.replicaType);
        }

    }
}
