using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using ReactiveUI;
using System.Collections.ObjectModel;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators;
using System.Reactive;
using TextGameRPG.Models.RegularDialogs;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class ItemInspectorViewModel : ViewModelBase
    {
        private ItemGeneratorBase _currentItem;
        private string? _header;

        public ItemGeneratorBase currentItem
        {
            get => _currentItem;
            set => this.RaiseAndSetIfChanged(ref _currentItem, value);
        }
        public string? header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }
        public ObservableCollection<ItemPropertyGeneratorBase> itemProperties { get; private set; } = new ObservableCollection<ItemPropertyGeneratorBase>();

        public ReactiveCommand<Unit, Unit> removeItemCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> editItemCommand { get; private set; }

        public ItemInspectorViewModel()
        {
            removeItemCommand = ReactiveCommand.Create(OnRemoveItemClick);
            editItemCommand = ReactiveCommand.Create(OnEditItemClick);
        }

        public void ShowItem(ItemGeneratorBase item)
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
            RegularDialogHelper.ShowConfirmDialog("Are you sure you want to delete this item?", () => 
            {
                var itemsDataBase = Scripts.GameCore.GameDataBase.GameDataBase.instance.itemsGenerators;
                itemsDataBase.RemoveData(_currentItem.id);
            });
        }

        private void OnEditItemClick()
        {
            //TODO
        }


    }
}
