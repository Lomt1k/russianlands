using System.Collections.Generic;
using System.Collections.ObjectModel;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using ReactiveUI;
using System.Linq;
using TextGameRPG.Views.Editor.ItemsEditor;
using System.Reactive;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.GameDataBase;

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

        public ReactiveCommand<Unit, Unit> addNewItemCommand { get; }

        public ItemsEditorViewModel()
        {
            categories = ItemCategory.GetAllCategories();
            _selectedCategory = categories[0];

            itemInspector = new ItemInspectorView();
            itemInspector.DataContext = itemInspectorViewModel = new ItemInspectorViewModel();

            addNewItemCommand = ReactiveCommand.Create(AddNewItem);

            RefreshShowedItems();
            GameDataBase.instance.itemsGenerators.onDataChanged += OnDataBaseChanged;
        }

        private void OnDataBaseChanged()
        {
            RefreshShowedItems();
        }

        private void RefreshShowedItems()
        {
            var items = GameDataBase.instance.itemsGenerators.GetAllData();

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

        private void AddNewItem()
        {
            var allItems = GameDataBase.instance.itemsGenerators.GetAllData().ToList();

            int newItemId = allItems.Count > 0 ? allItems.Max(x => x.id) + 1 : 1;
            var newItemType = selectedCategory.itemType == ItemType.Any
                ? ItemType.MeleeWeapon
                : selectedCategory.itemType;

            var newItem = new ItemGeneratorBase("[NEW ITEM]", newItemId, newItemType, ItemRarity.Common, 0);
            GameDataBase.instance.itemsGenerators.AddData(newItemId, newItem);
            selectedItem = newItem;

            var inspectorViewModel = itemInspector.DataContext as ItemInspectorViewModel;
            inspectorViewModel.StartEditItem(isNewItem: true);
        }

    }
}
