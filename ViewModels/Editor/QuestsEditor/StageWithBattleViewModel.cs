using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Quests.QuestStages;
using ReactiveUI;

namespace TextGameRPG.ViewModels.Editor.QuestsEditor
{
    public class StageWithBattleViewModel : ViewModelBase
    {
        private Dictionary<string, int> _mobIds = new Dictionary<string, int>();
        private string? _selectedMob;

        public QuestStageWithBattle stage { get; }
        public ObservableCollection<string> mobsList { get; }
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

        public StageWithBattleViewModel(QuestStageWithBattle stage)
        {
            this.stage = stage;

            var mobDB = Scripts.GameCore.GameDataBase.GameDataBase.instance.mobs;
            mobsList = new ObservableCollection<string>();
            foreach (var mob in mobDB.GetAllData())
            {
                var mobStr = $"{mob.id} | {mob.debugName}";
                _mobIds[mobStr] = mob.id;
                mobsList.Add(mobStr);

                if (mob.id == stage.mobId)
                {
                    selectedMob = mobStr;
                }
            }
        }

    }
}
