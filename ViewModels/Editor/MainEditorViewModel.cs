using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using MarkOne.Models.Editor;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Views.Editor.BuildingsEditor;
using MarkOne.Views.Editor.ItemsEditor;
using MarkOne.Views.Editor.LocationMobsEditor;
using MarkOne.Views.Editor.MobsEditor;
using MarkOne.Views.Editor.PotionsEditor;
using MarkOne.Views.Editor.QuestsEditor;

namespace MarkOne.ViewModels.Editor;

public class MainEditorViewModel : ViewModelBase
{
    private readonly GameDataHolder gameDataHolder = Services.Get<GameDataHolder>();

    private MainEditorCategory _selectedCategory;

    public ObservableCollection<MainEditorCategory> categories { get; } = new();
    public MainEditorCategory selectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    public ReactiveCommand<Unit, Unit> saveCommand { get; }
    public ReactiveCommand<Unit, Unit> reloadCommand { get; }

    public MainEditorViewModel()
    {
        RestartEditor();

        saveCommand = ReactiveCommand.Create(SaveCommand);
        reloadCommand = ReactiveCommand.Create(ReloadCommand);

        gameDataHolder.onDataReloaded += OnDataReloaded;
    }

    private void RestartEditor()
    {
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        categories.Clear();
        categories.Add(new MainEditorCategory("Buildings", new BuildingsEditorView()));
        categories.Add(new MainEditorCategory("Items", new ItemsEditorView()));
        categories.Add(new MainEditorCategory("Quests", new QuestsEditorView()));
        categories.Add(new MainEditorCategory("Mobs", new MobsEditorView()));
        categories.Add(new MainEditorCategory("LocationMobs", new LocationMobsEditorView()));
        categories.Add(new MainEditorCategory("Potions", new PotionsEditorView()));
    }

    private void SaveCommand()
    {
        gameDataHolder.SaveAllData();
    }

    private void ReloadCommand()
    {
        gameDataHolder.LoadAllData();
    }

    private void OnDataReloaded()
    {
        RestartEditor();
    }


}
