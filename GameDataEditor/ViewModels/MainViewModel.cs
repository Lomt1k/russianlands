using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using GameDataEditor.Models.Editor;
using GameDataEditor.Views.Editor.BuildingsEditor;
using GameDataEditor.Views.Editor.ItemsEditor;
using GameDataEditor.Views.Editor.QuestsEditor;
using GameDataEditor.Views.Editor.MobsEditor;
using GameDataEditor.Views.Editor.LocationMobsEditor;
using GameDataEditor.Views.Editor.PotionsEditor;
using GameDataEditor.Views.Editor.ArenaSettingsEditor;
using GameDataEditor.Views.Editor.ArenaLeaguesEditor;
using GameDataEditor.Views.Editor.ArenaShopSettingsEditor;
using GameDataEditor.Views.Editor.ShopSettingsEditor;
using GameDataEditor.Views.Editor.OffersEditor;

namespace GameDataEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private MainEditorCategory _selectedCategory;

    public ObservableCollection<MainEditorCategory> categories { get; } = new();
    public MainEditorCategory selectedCategory
    {
        get => _selectedCategory;
        set => this.RaiseAndSetIfChanged(ref _selectedCategory, value);
    }

    public ReactiveCommand<Unit, Unit> saveCommand { get; }
    public ReactiveCommand<Unit, Unit> reloadCommand { get; }

    public MainViewModel()
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
        categories.Add(new MainEditorCategory("Location Mobs", new LocationMobsEditorView()));
        categories.Add(new MainEditorCategory("Potions", new PotionsEditorView()));
        categories.Add(new MainEditorCategory("Arena Settings", new ArenaSettingsEditorView()));
        categories.Add(new MainEditorCategory("Arena Leagues", new ArenaLeaguesEditorView()));
        categories.Add(new MainEditorCategory("Arena Shop Settings", new ArenaShopSettingsEditorView()));
        categories.Add(new MainEditorCategory("Shop Settings", new ShopSettingsEditorView()));
        categories.Add(new MainEditorCategory("Offers", new OffersEditorView()));
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
