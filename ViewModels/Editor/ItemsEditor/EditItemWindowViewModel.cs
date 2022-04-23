using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemWindowViewModel : ViewModelBase
    {
        private EditItemWindow _window;
        private EnumValueModel<ItemType> _selectedItemType;
        private EnumValueModel<ItemRarity> _selectedItemRarity;
        private ItemPropertyGeneratorBase? _selectedProperty;

        public ItemGeneratorBase editableItem { get; }
        public ObservableCollection<EnumValueModel<ItemType>> itemTypeList { get; }
        public ObservableCollection<EnumValueModel<ItemRarity>> itemRarityList { get; }

        public EnumValueModel<ItemType> selectedItemType
        {
            get => _selectedItemType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItemType, value);
                editableItem.itemType = value.value;
            }
        }

        public EnumValueModel<ItemRarity> selectedItemRarity
        {
            get => _selectedItemRarity;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItemRarity, value);
                editableItem.itemRarity = value.value;
            }
        }

        public ItemPropertyGeneratorBase? selectedProperty
        {
            get => _selectedProperty;
            set => this.RaiseAndSetIfChanged(ref _selectedProperty, value);
        }

        private System.Action closeWindow { get; }
        public ReactiveCommand<Unit, Unit> addPropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> editPropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> removePropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemWindowViewModel(EditItemWindow window, ItemGeneratorBase item)
        {
            window.Title = $"{item.debugName} [ID {item.id}]";
            _window = window;

            editableItem = item.Clone();
            itemTypeList = EnumValueModel<ItemType>.CreateCollection(excludeValue: ItemType.Any);
            itemRarityList = EnumValueModel<ItemRarity>.CreateCollection();

            _selectedItemType = EnumValueModel<ItemType>.GetModel(itemTypeList, editableItem.itemType);
            _selectedItemRarity = EnumValueModel<ItemRarity>.GetModel(itemRarityList, editableItem.itemRarity);

            closeWindow = () => window.Close();
            addPropertyCommand = ReactiveCommand.Create(AddNewProperty);
            editPropertyCommand = ReactiveCommand.Create(EditSelectedProperty);
            removePropertyCommand = ReactiveCommand.Create(RemoveSelectedProperty);
            saveCommand = ReactiveCommand.Create(SaveItemChanges + closeWindow);
            cancelCommand = ReactiveCommand.Create(closeWindow);
        }

        private void AddNewProperty()
        {
            //TODO
        }

        private void EditSelectedProperty()
        {
            var window = new EditItemPropertyWindow();
            window.DataContext = new EditItemPropertyWindowViewModel(window, _selectedProperty, (modifiedProperty) => 
            {
                var index = Array.IndexOf(editableItem.properties, _selectedProperty);
                editableItem.properties[index] = modifiedProperty;
            });
            window.ShowDialog(_window);
        }

        private void RemoveSelectedProperty()
        {
            //TODO
        }

        private void SaveItemChanges()
        {
            var dataBase = Scripts.GameCore.GameDataBase.GameDataBase.instance.itemsGenerators;
            dataBase.ChangeData(editableItem.id, editableItem);
        }

    }
}
