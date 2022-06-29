using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using ReactiveUI;
using System.Reactive;
using System.Linq;

namespace TextGameRPG.ViewModels.Editor.MobsEditor
{
    public class MobsEditorViewModel : ViewModelBase
    {
        private MobData? _selectedMob;

        public DataDictionaryWithIntegerID<MobData> mobsDB => GameDataBase.instance.mobs;
        public ObservableCollection<MobData> mobsList { get; } = new ObservableCollection<MobData>();
        public MobData? selectedMob
        {
            get => _selectedMob;
            set => this.RaiseAndSetIfChanged(ref _selectedMob, value);
        }

        public ReactiveCommand<Unit,Unit> addMobCommand { get; }
        public ReactiveCommand<Unit, Unit> removeMobCommand { get; }

        public MobsEditorViewModel()
        {
            RefreshMobsList();
            addMobCommand = ReactiveCommand.Create(AddNewMob);
            removeMobCommand = ReactiveCommand.Create(RemoveSelectedMob);
        }

        private void RefreshMobsList()
        {
            mobsList.Clear();
            foreach (var mobData in mobsDB.GetAllData())
            {
                mobsList.Add(mobData);
            }
        }

        private void AddNewMob()
        {
            var allMobs = mobsDB.GetAllData();
            var id = mobsDB.count > 0 ? allMobs.Max(x => x.id) + 1 : 1;
            var mobData = new MobData()
            {
                id = id,
            };
            mobsDB.AddData(id, mobData);
            RefreshMobsList();
        }

        private void RemoveSelectedMob()
        {
            if (_selectedMob == null)
                return;

            mobsDB.RemoveData(_selectedMob.id);
            _selectedMob = null;
            RefreshMobsList();
        }

    }
}
