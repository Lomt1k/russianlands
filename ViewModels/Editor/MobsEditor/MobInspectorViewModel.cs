using TextGameRPG.Scripts.GameCore.Units.Mobs;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Items;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Views.UserControls;
using TextGameRPG.Models.UserControls;
using System.Reactive;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.ViewModels.Editor.MobsEditor
{
    public class MobInspectorViewModel : ViewModelBase
    {
        private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();


        private MobData? _mob;
        private string _header = string.Empty;
        private EnumValueModel<MobType>? _selectedMobType;
        private ObjectPropertiesEditorView? _statsSettingsView;
        private ObjectPropertiesEditorView? _selectedAttackView;
        private MobsEditorViewModel _mobEditorVM;

        public MobData? mob
        {
            get => _mob;
            set => this.RaiseAndSetIfChanged(ref _mob, value);
        }
        public string header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }
        public ObservableCollection<EnumValueModel<MobType>> mobTypes { get; }
        public ObservableCollection<EnumValueModel<Rarity>> rarities { get; }
        public ObservableCollection<ObjectPropertiesEditorView> attackViews { get; }

        public EnumValueModel<MobType>? selectedMobType
        {
            get => _selectedMobType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedMobType, value);
                if (_mob != null && value != null)
                {
                    _mob.mobType = value.value;
                }
            }
        }

        public ObjectPropertiesEditorView? statsSettingsView
        {
            get => _statsSettingsView;
            set => this.RaiseAndSetIfChanged(ref _statsSettingsView, value);
        }
        public ObjectPropertiesEditorView? selectedAttackView
        {
            get => _selectedAttackView;
            set => this.RaiseAndSetIfChanged(ref _selectedAttackView, value);
        }

        public ReactiveCommand<Unit, Unit> addAttackCommand { get; }
        public ReactiveCommand<Unit, Unit> removeAttackCommand { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public MobInspectorViewModel(MobsEditorViewModel parent)
        {
            mobTypes = EnumValueModel<MobType>.CreateCollection();
            rarities = EnumValueModel<Rarity>.CreateCollection();

            attackViews = new ObservableCollection<ObjectPropertiesEditorView>();
            addAttackCommand = ReactiveCommand.Create(AddNewAttack);
            removeAttackCommand = ReactiveCommand.Create(RemoveSelectedAttack);
            saveCommand = ReactiveCommand.Create(SaveMobChanges);
            cancelCommand = ReactiveCommand.Create(ResetMobChanges);

            _mobEditorVM = parent;
        }

        public void ShowMob(MobData data)
        {
            mob = data;
            header = $"{data.debugName} [ID: {data.id}]";
            selectedMobType = mobTypes.First(x => x.value == data.mobType);
            statsSettingsView = UserControlsHelper.CreateObjectEditorView(data.statsSettings);

            UserControlsHelper.RefillObjectEditorsCollection(attackViews, data.mobAttacks);
        }

        private void AddNewAttack()
        {
            if (mob == null)
                return;

            _mob.mobAttacks.Add(new MobAttack());
            UserControlsHelper.RefillObjectEditorsCollection(attackViews, _mob.mobAttacks);
        }

        private void RemoveSelectedAttack()
        {
            if (_mob == null || _selectedAttackView == null)
                return;

            var mobAttack = _selectedAttackView.vm.GetEditableObject<MobAttack>();
            _mob.mobAttacks.Remove(mobAttack);
            UserControlsHelper.RefillObjectEditorsCollection(attackViews, _mob.mobAttacks);
        }

        private void SaveMobChanges()
        {
            if (mob == null)
                return;

            //_statsSettingsView?.vm.SaveObjectChanges();
            foreach (var attackView in attackViews)
            {
                //attackView.vm.SaveObjectChanges();
            }

            gameDataBase.mobs.ChangeData(mob.id, mob);

            _mobEditorVM.RefreshMobsList();
            _mobEditorVM.selectedMob = mob;
        }

        private void ResetMobChanges()
        {
            gameDataBase.mobs.ReloadAllData();

            _mobEditorVM.RefreshMobsList();
            _mobEditorVM.selectedMob = mob;
        }

    }
}
