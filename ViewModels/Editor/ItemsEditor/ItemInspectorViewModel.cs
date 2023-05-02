using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TextGameRPG.Models.RegularDialogs;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor;

public class ItemInspectorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();


    private ItemData _currentItem;
    private string? _header;
    private bool _hasAbilities;
    private bool _hasProperties;

    public ItemData currentItem
    {
        get => _currentItem;
        set => this.RaiseAndSetIfChanged(ref _currentItem, value);
    }
    public string? header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }
    public bool hasAbilities
    {
        get => _hasAbilities;
        set => this.RaiseAndSetIfChanged(ref _hasAbilities, value);
    }
    public bool hasProperties
    {
        get => _hasProperties;
        set => this.RaiseAndSetIfChanged(ref _hasProperties, value);
    }

    public ObservableCollection<ItemAbilityBase> itemAbilities { get; private set; } = new ObservableCollection<ItemAbilityBase>();
    public ObservableCollection<ItemPropertyBase> itemProperties { get; private set; } = new ObservableCollection<ItemPropertyBase>();

    public ReactiveCommand<Unit, Unit> removeItemCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> editItemCommand { get; private set; }

    public ItemInspectorViewModel()
    {
        removeItemCommand = ReactiveCommand.Create(OnRemoveItemClick);
        editItemCommand = ReactiveCommand.Create(OnEditItemClick);
    }

    public void ShowItem(ItemData item)
    {
        currentItem = item;
        RefreshHeader();
        RefreshItemAbilities();
        RefreshItemProperties();
    }

    public void RefreshHeader()
    {
        header = $"{currentItem.debugName} ({currentItem.itemRarity}, Lvl {currentItem.requiredLevel})";
    }

    private void RefreshItemAbilities()
    {
        itemAbilities.Clear();
        foreach (var item in currentItem.abilities)
        {
            itemAbilities.Add(item);
        }
        hasAbilities = itemAbilities.Count > 0;
    }

    private void RefreshItemProperties()
    {
        itemProperties.Clear();
        foreach (var item in currentItem.properties)
        {
            itemProperties.Add(item);
        }
        hasProperties = itemProperties.Count > 0;
    }

    private void OnRemoveItemClick()
    {
        RegularDialogHelper.ShowConfirmDialog("Are you sure you want to delete this item?" +
            $"\n\n[ID {_currentItem.id}] {_currentItem.debugName} ({_currentItem.itemRarity}, Lvl {_currentItem.requiredLevel})", () =>
        {
            gameDataBase.items.RemoveData(_currentItem.id);
        });
    }

    private void OnEditItemClick()
    {
        StartEditItem(isNewItem: false);
    }

    public void StartEditItem(bool isNewItem)
    {
        var editWindow = new EditItemWindow();
        editWindow.DataContext = new EditItemWindowViewModel(editWindow, _currentItem, isNewItem);
        editWindow.ShowDialog(Program.mainWindow);
    }


}
