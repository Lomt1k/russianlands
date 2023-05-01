using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using ReactiveUI;
using System.Reactive;
using System.Linq;
using TextGameRPG.Views.Editor.MobsEditor;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.ViewModels.Editor.MobsEditor
{
    public class MobsEditorViewModel : ViewModelBase
    {
        private MobData? _selectedMob;
        private MobInspectorViewModel _mobInspectorVM;

        private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();

        public ObservableCollection<MobData> mobsList { get; } = new ObservableCollection<MobData>();
        public MobData? selectedMob
        {
            get => _selectedMob;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMob, value);
                ShowMob(_selectedMob);
            }
        }
        public MobInspectorView mobInspector { get; }

        public ReactiveCommand<Unit,Unit> addMobCommand { get; }
        public ReactiveCommand<Unit, Unit> removeMobCommand { get; }

        public MobsEditorViewModel()
        {
            RefreshMobsList();
            addMobCommand = ReactiveCommand.Create(AddNewMob);
            removeMobCommand = ReactiveCommand.Create(RemoveSelectedMob);

            mobInspector = new MobInspectorView();
            mobInspector.DataContext = _mobInspectorVM = new MobInspectorViewModel();
        }

        public void RefreshMobsList()
        {
            mobsList.Clear();
            foreach (var mobData in gameDataBase.mobs.GetAllData())
            {
                mobsList.Add(mobData);
            }
        }

        private void AddNewMob()
        {
            var allMobs = gameDataBase.mobs.GetAllData();
            var id = gameDataBase.mobs.count > 0 ? allMobs.Max(x => x.id) + 1 : 1;
            var mobData = new MobData()
            {
                id = id,
            };
            gameDataBase.mobs.AddData(id, mobData);
            RefreshMobsList();
        }

        private void RemoveSelectedMob()
        {
            if (_selectedMob == null)
                return;

            gameDataBase.mobs.RemoveData(_selectedMob.id);
            _selectedMob = null;
            RefreshMobsList();
        }

        private void ShowMob(MobData? mobData)
        {
            if (mobData == null)
                return;

            _mobInspectorVM.ShowMob(mobData);
        }

    }
}
