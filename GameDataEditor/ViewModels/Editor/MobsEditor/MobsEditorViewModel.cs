using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Units.Mobs;
using MarkOne.Views.Editor.MobsEditor;

namespace GameDataEditor.ViewModels.Editor.MobsEditor;

public class MobsEditorViewModel : ViewModelBase
{
    private QuestMobData? _selectedMob;
    private readonly MobInspectorViewModel _mobInspectorVM;

    private static readonly GameDataHolder gameDataBase = ServiceLocator.Get<GameDataHolder>();

    public ObservableCollection<QuestMobData> mobsList { get; } = new ObservableCollection<QuestMobData>();
    public QuestMobData? selectedMob
    {
        get => _selectedMob;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMob, value);
            ShowMob(_selectedMob);
        }
    }
    public MobInspectorView mobInspector { get; }

    public ReactiveCommand<Unit, Unit> addMobCommand { get; }
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
        var mobData = new QuestMobData()
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

    private void ShowMob(QuestMobData? mobData)
    {
        if (mobData == null)
            return;

        _mobInspectorVM.ShowMob(mobData);
    }

}
