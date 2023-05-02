using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TextGameRPG.Models;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Views.UserControls;

namespace TextGameRPG.ViewModels.Editor.MobsEditor;

public class MobInspectorViewModel : ViewModelBase
{
    private QuestMobData? _mob;
    private string _header = string.Empty;
    private ObjectPropertiesEditorView? _statsSettingsView;
    private ObjectPropertiesEditorView? _selectedAttackView;

    public QuestMobData? mob
    {
        get => _mob;
        set => this.RaiseAndSetIfChanged(ref _mob, value);
    }
    public string header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }
    public ObservableCollection<EnumValueModel<Rarity>> rarities { get; }
    public ObservableCollection<ObjectPropertiesEditorView> attackViews { get; }

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

    public MobInspectorViewModel()
    {
        rarities = EnumValueModel<Rarity>.CreateCollection();
        attackViews = new ObservableCollection<ObjectPropertiesEditorView>();
        addAttackCommand = ReactiveCommand.Create(AddNewAttack);
        removeAttackCommand = ReactiveCommand.Create(RemoveSelectedAttack);
    }

    public void ShowMob(QuestMobData data)
    {
        mob = data;
        header = $"{data.debugName} [ID: {data.id}]";
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
