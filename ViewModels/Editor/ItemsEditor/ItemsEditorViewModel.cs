using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor;

public class ItemsEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();

    private ItemCategory _selectedCategory;
    private ItemData? _selectedItem;

    public ObservableCollection<ItemCategory> categories { get; }
    public ObservableCollection<ItemData> showedItems { get; private set; } = new ObservableCollection<ItemData>();
    public ItemCategory selectedCategory
    {
        get => _selectedCategory;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedCategory, value);
            RefreshShowedItems();
        }
    }
    public ItemData? selectedItem
    {
        get => _selectedItem;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedItem, value);
            if (_selectedItem != null)
            {
                itemInspectorViewModel.ShowItem(_selectedItem);
            }
        }
    }
    public ItemInspectorView itemInspector { get; }
    public ItemInspectorViewModel itemInspectorViewModel { get; }

    public ReactiveCommand<Unit, Unit> addNewItemCommand { get; }

    public ItemsEditorViewModel()
    {
        categories = ItemCategory.GetAllCategories();
        _selectedCategory = categories[0];

        itemInspector = new ItemInspectorView();
        itemInspector.DataContext = itemInspectorViewModel = new ItemInspectorViewModel();

        addNewItemCommand = ReactiveCommand.Create(AddNewItem);

        RefreshShowedItems();
        gameDataBase.items.onDataChanged += OnDataBaseChanged;
    }

    private void OnDataBaseChanged()
    {
        RefreshShowedItems();
    }

    private void RefreshShowedItems()
    {
        var items = gameDataBase.items.GetAllData();

        if (_selectedCategory != null && _selectedCategory.itemType != ItemType.Any)
        {
            items = items.Where(x => x.itemType == _selectedCategory.itemType);
        }

        RefreshShowedItems(items);
    }

    private void RefreshShowedItems(IEnumerable<ItemData> items)
    {
        showedItems.Clear();
        foreach (var item in items)
        {
            showedItems.Add(item);
        }
    }

    private void AddNewItem()
    {
        var allItems = gameDataBase.items.GetAllData().ToList();

        var newItemId = allItems.Count > 0 ? allItems.Max(x => x.id) + 1 : 1;
        var newItemType = selectedCategory.itemType == ItemType.Any
            ? ItemType.Sword
            : selectedCategory.itemType;

        var newItem = new ItemData("[NEW ITEM]", newItemId, newItemType);
        gameDataBase.items.AddData(newItemId, newItem);
        selectedItem = newItem;

        var inspectorViewModel = itemInspector.DataContext as ItemInspectorViewModel;
        inspectorViewModel.StartEditItem(isNewItem: true);
    }

}
