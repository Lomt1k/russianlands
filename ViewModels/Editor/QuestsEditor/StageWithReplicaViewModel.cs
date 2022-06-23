using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using ReactiveUI;
using System.Linq;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithReplicaViewModel : ViewModelBase
    {
        private EnumValueModel<CharacterType> _selectedCharacter;

        public QuestStageWithReplica stage { get; }
        public ObservableCollection<EnumValueModel<CharacterType>> characters { get; }
        public EnumValueModel<CharacterType> selectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCharacter, value);
                stage.replica.characterType = value.value;
            }
        }

        public StageWithReplicaViewModel(QuestStageWithReplica stage)
        {
            this.stage = stage;
            characters = EnumValueModel<CharacterType>.CreateCollection();
            _selectedCharacter = characters.First(x => x.value == stage.replica.characterType);
        }

    }
}
