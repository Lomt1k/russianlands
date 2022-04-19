using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using ReactiveUI;
using System.Linq;
using TextGameRPG.Views.Editor.ItemsEditor;

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

        public ItemsEditorViewModel()
        {
            categories = ItemCategory.GetAllCategories();
            _selectedCategory = categories[0];

            itemInspector = new ItemInspectorView();
            itemInspector.DataContext = itemInspectorViewModel = new ItemInspectorViewModel();

            RefreshShowedItems();
        }

        private void RefreshShowedItems()
        {
            var items = Scripts.GameCore.GameDataBase.GameDataBase.instance.itemsGenerators.GetAllData();

            if (_selectedCategory != null && _selectedCategory.itemType != Scripts.GameCore.Items.ItemType.Any)
            {
                items = items.Where(x => x.itemType == _selectedCategory.itemType);
            }

            showedItems = new ObservableCollection<ItemGeneratorBase>(items);
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
