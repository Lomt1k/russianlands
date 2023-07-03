using GameDataEditor.ViewModels.Editor.ShopItems;
using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace GameDataEditor.ViewModels.Editor.ArenaShopSettingsEditor;
internal class ArenaShopSettingsEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private byte? _selectedTownhall;
    private ArenaShopSettings? _arenaShopSettings;

    public ObservableCollection<byte> townhallList { get; } = new();
    public byte? selectedTownhall
    {
        get => _selectedTownhall;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTownhall, value);
            if (value != null)
            {
                SetModel(value.Value);
            }
        }
    }
    public EditorListView mainCategoryListView { get; set; } = new() { DataContext = new EditorShopItemsListViewModel() };
    public EditorListView exchangeCategoryListView { get; set; } = new() { DataContext = new EditorShopItemsListViewModel() };
    public EditorListView itemPatternsListView { get; set; } = new() { DataContext = new ArenaShopItemPatternsListViewModel() };
    public ArenaShopSettings? arenaShopSettings
    {
        get => _arenaShopSettings;
        set => this.RaiseAndSetIfChanged(ref _arenaShopSettings, value);
    }

    public ReactiveCommand<Unit, Unit> addTownhallSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> removeTownhallSettingsCommand { get; }

    public ArenaShopSettingsEditorViewModel()
    {
        RefreshTownhallList();
        addTownhallSettingsCommand = ReactiveCommand.Create(AddNewTownhall);
        removeTownhallSettingsCommand = ReactiveCommand.Create(RemoveSelectedTownhall);
    }

    private void RefreshTownhallList()
    {
        townhallList.Clear();
        foreach (var arenaShopSettings in gameDataHolder.arenaShopSettings.GetAllData())
        {
            townhallList.Add(arenaShopSettings.id);
        }
    }

    private void SetModel(byte townhallLevel)
    {
        arenaShopSettings = gameDataHolder.arenaShopSettings[townhallLevel];
        if (mainCategoryListView.DataContext is EditorShopItemsListViewModel mainCategoryListViewModel)
        {
            mainCategoryListViewModel.SetModel(arenaShopSettings.mainCategoryItems);
        }
        if (exchangeCategoryListView.DataContext is EditorShopItemsListViewModel exchangeCategoryListViewModel)
        {
            exchangeCategoryListViewModel.SetModel(arenaShopSettings.exchangeCategoryItems);
        }
        if (itemPatternsListView.DataContext is ArenaShopItemPatternsListViewModel itemPatternsListViewModel)
        {
            itemPatternsListViewModel.SetModel(arenaShopSettings.tempItemPatterns);
        }
    }

    private void AddNewTownhall()
    {
        var maxTownhall = townhallList.Count > 0 ? townhallList.Max() : 0;
        var newTownhall = (byte)(maxTownhall + 1);
        gameDataHolder.arenaShopSettings.AddData(newTownhall, new ArenaShopSettings() { id = newTownhall });
        RefreshTownhallList();
        selectedTownhall = newTownhall;
    }

    private void RemoveSelectedTownhall()
    {
        if (selectedTownhall is null)
            return;

        gameDataHolder.arenaShopSettings.RemoveData(selectedTownhall.Value);
        RefreshTownhallList();
        selectedTownhall = null;
    }

}
