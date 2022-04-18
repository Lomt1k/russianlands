using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using ReactiveUI;
using System.Linq;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class ItemsEditorViewModel : ViewModelBase
    {
        private ItemCategory _selectedCategory;
        private ItemGeneratorBase? _selectedItem;

        public ObservableCollection<ItemCategory> categories { get; }
        public ObservableCollection<ItemGeneratorBase> showedItems { get; private set; } = new ObservableCollection<ItemGeneratorBase>();
        public ItemCategory selectedCategory 
        {
            get => _selectedCategory;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedCategory, value);
                RefreshShowedItems();
            }
        }
        public ItemGeneratorBase? selectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public ItemsEditorViewModel()
        {
            categories = ItemCategory.GetAllCategories();
            _selectedCategory = categories[0];

            RefreshShowedItems();
        }

        private void RefreshShowedItems()
        {
            var items = Scripts.GameCore.GameDataBase.GameDataBase.instance.itemsGenerators.GetAllData();

            if (_selectedCategory != null && _selectedCategory.itemType != Scripts.GameCore.Items.ItemType.Any)
            {
                items = items.Where(x => x.itemType == _selectedCategory.itemType);
            }

            RefreshShowedItems(items);
        }

        private void RefreshShowedItems(IEnumerable<ItemGeneratorBase> items)
        {
            showedItems.Clear();
            foreach (var item in items)
            {
                showedItems.Add(item);
            }
        }

    }
}
