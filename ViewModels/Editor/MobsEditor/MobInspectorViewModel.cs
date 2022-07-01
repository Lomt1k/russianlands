using TextGameRPG.Scripts.GameCore.Units.Mobs;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Items;
using System.Collections.ObjectModel;
using System.Linq;
using TextGameRPG.Views.Editor.UserControls;
using TextGameRPG.Models.UserControls;
using System.Reactive;

namespace TextGameRPG.ViewModels.Editor.MobsEditor
{
    public class MobInspectorViewModel : ViewModelBase
    {
        private MobData? _mob;
        private string _header = string.Empty;
        private EnumValueModel<MobType>? _selectedMobType;
        private EnumValueModel<Rarity>? _selectedRarity;
        private ObjectFieldsEditorView? _encounterSettingsView;
        private ObjectFieldsEditorView? _statsSettingsView;
        private ObjectFieldsEditorView? _selectedAttackView;

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
        public ObservableCollection<ObjectFieldsEditorView> attackViews { get; }

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
        public EnumValueModel<Rarity>? selectedRarity
        {
            get => _selectedRarity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedRarity, value);
                if (_mob != null && value != null)
                {
                    _mob.rarity = value.value;
                }
            }
        }
        public ObjectFieldsEditorView? encounterSettingsView
        {
            get => _encounterSettingsView;
            set => this.RaiseAndSetIfChanged(ref _encounterSettingsView, value);
        }
        public ObjectFieldsEditorView? statsSettingsView
        {
            get => _statsSettingsView;
            set => this.RaiseAndSetIfChanged(ref _statsSettingsView, value);
        }
        public ObjectFieldsEditorView? selectedAttackView
        {
            get => _selectedAttackView;
            set => this.RaiseAndSetIfChanged(ref _selectedAttackView, value);
        }

        public ReactiveCommand<Unit, Unit> addAttackCommand { get; }
        public ReactiveCommand<Unit, Unit> removeAttackCommand { get; }

        public MobInspectorViewModel()
        {
            mobTypes = EnumValueModel<MobType>.CreateCollection();
            rarities = EnumValueModel<Rarity>.CreateCollection();

            attackViews = new ObservableCollection<ObjectFieldsEditorView>();
            addAttackCommand = ReactiveCommand.Create(AddNewAttack);
            removeAttackCommand = ReactiveCommand.Create(RemoveSelectedAttack);
        }

        public void ShowMob(MobData data)
        {
            mob = data;
            header = $"{data.debugName} [ID: {data.id}]";
            selectedMobType = mobTypes.First(x => x.value == data.mobType);
            selectedRarity = rarities.First(x => x.value == data.rarity);
            encounterSettingsView = UserControlsHelper.CreateObjectEditorView(data.encounterSettings);
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

    }
}
