using GameDataEditor.ViewModels.Editor.ShopItems;
using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Scripts.GameCore.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using MarkOne.Scripts.GameCore.Shop;

namespace GameDataEditor.ViewModels.Editor.ShopSettingsEditor;
internal class ShopSettingsEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private byte? _selectedTownhall;
    private ShopSettings? _shopSettings;

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
    public EditorListView premiumCategoryListView { get; set; } = new() { DataContext = new EditorShopItemsListViewModel() };
    public EditorListView lootboxCategoryListView { get; set; } = new() { DataContext = new EditorShopItemsListViewModel() };
    public EditorListView diamondsCategoryListView { get; set; } = new() { DataContext = new EditorShopItemsListViewModel() };
    public ShopSettings? shopSettings
    {
        get => _shopSettings;
        set => this.RaiseAndSetIfChanged(ref _shopSettings, value);
    }

    public ReactiveCommand<Unit, Unit> addTownhallSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> removeTownhallSettingsCommand { get; }

    public ShopSettingsEditorViewModel()
    {
        RefreshTownhallList();
        addTownhallSettingsCommand = ReactiveCommand.Create(AddNewTownhall);
        removeTownhallSettingsCommand = ReactiveCommand.Create(RemoveSelectedTownhall);
    }

    private void RefreshTownhallList()
    {
        townhallList.Clear();
        foreach (var shopSettings in gameDataHolder.shopSettings.GetAllData())
        {
            townhallList.Add(shopSettings.id);
        }
    }

    private void SetModel(byte townhallLevel)
    {
        shopSettings = gameDataHolder.shopSettings[townhallLevel];
        if (premiumCategoryListView.DataContext is EditorShopItemsListViewModel premiumCategoryListViewModel)
        {
            premiumCategoryListViewModel.SetModel(shopSettings.premiumCategoryItems);
        }
        if (lootboxCategoryListView.DataContext is EditorShopItemsListViewModel lootboxCategoryListViewModel)
        {
            lootboxCategoryListViewModel.SetModel(shopSettings.lootboxCategoryItems);
        }
        if (diamondsCategoryListView.DataContext is EditorShopItemsListViewModel diamondsCategoryListViewModel)
        {
            diamondsCategoryListViewModel.SetModel(shopSettings.diamondsCategoryItems);
        }
    }

    private void AddNewTownhall()
    {
        var maxTownhall = townhallList.Count > 0 ? townhallList.Max() : 0;
        var newTownhall = (byte)(maxTownhall + 1);
        gameDataHolder.shopSettings.AddData(newTownhall, new ShopSettings() { id = newTownhall });
        RefreshTownhallList();
        selectedTownhall = newTownhall;
    }

    private void RemoveSelectedTownhall()
    {
        if (selectedTownhall is null)
            return;

        gameDataHolder.shopSettings.RemoveData(selectedTownhall.Value);
        RefreshTownhallList();
        selectedTownhall = null;
    }

}
