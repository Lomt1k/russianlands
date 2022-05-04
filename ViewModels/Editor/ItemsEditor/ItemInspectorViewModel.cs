using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using TextGameRPG.Models.RegularDialogs;
using TextGameRPG.Views.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class ItemInspectorViewModel : ViewModelBase
    {
        private ItemBase _currentItem;
        private string? _header;

        public ItemBase currentItem
        {
            get => _currentItem;
            set => this.RaiseAndSetIfChanged(ref _currentItem, value);
        }
        public string? header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }
        public ObservableCollection<ItemPropertyBase> itemProperties { get; private set; } = new ObservableCollection<ItemPropertyBase>();

        public ReactiveCommand<Unit, Unit> removeItemCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> editItemCommand { get; private set; }

        public ItemInspectorViewModel()
        {
            removeItemCommand = ReactiveCommand.Create(OnRemoveItemClick);
            editItemCommand = ReactiveCommand.Create(OnEditItemClick);
        }

        public void ShowItem(ItemBase item)
        {
            currentItem = item;
            RefreshHeader();
            RefreshItemProperties();
        }

        public void RefreshHeader()
        {
            header = $"{currentItem.debugName} ({currentItem.itemRarity}, Lvl {currentItem.requiredLevel})";
        }

        private void RefreshItemProperties()
        {
            itemProperties.Clear();
            foreach (var item in currentItem.properties)
            {
                itemProperties.Add(item);
            }
        }

        private void OnRemoveItemClick()
        {
            RegularDialogHelper.ShowConfirmDialog("Are you sure you want to delete this item?" +
                $"\n\n[ID {_currentItem.id}] {_currentItem.debugName} ({_currentItem.itemRarity}, Lvl {_currentItem.requiredLevel})", () => 
            {
                var itemsDataBase = Scripts.GameCore.GameDataBase.GameDataBase.instance.items;
                itemsDataBase.RemoveData(_currentItem.id);
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
}
