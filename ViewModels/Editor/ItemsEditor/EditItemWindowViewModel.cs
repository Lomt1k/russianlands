﻿using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemWindowViewModel : ViewModelBase
    {
        private EditItemWindow _window;
        private bool _isNewItem;
        private EnumValueModel<ItemType> _selectedItemType;
        private EnumValueModel<ItemRarity> _selectedItemRarity;
        private ItemAbilityBase? _selectedAbility;
        private ItemPropertyBase? _selectedProperty;

        public ItemData editableItem { get; }
        public ObservableCollection<EnumValueModel<ItemType>> itemTypeList { get; }
        public ObservableCollection<EnumValueModel<ItemRarity>> itemRarityList { get; }
        public ObservableCollection<ItemAbilityBase> itemAbilities { get; }
        public ObservableCollection<ItemPropertyBase> itemProperties { get; }

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

        public ItemAbilityBase? selectedAbility
        {
            get => _selectedAbility;
            set => this.RaiseAndSetIfChanged(ref _selectedAbility, value);
        }

        public ItemPropertyBase? selectedProperty
        {
            get => _selectedProperty;
            set => this.RaiseAndSetIfChanged(ref _selectedProperty, value);
        }

        public ReactiveCommand<Unit, Unit> addAbilityCommand { get; }
        public ReactiveCommand<Unit, Unit> editAbilityCommand { get; }
        public ReactiveCommand<Unit, Unit> removeAbilityCommand { get; }
        public ReactiveCommand<Unit, Unit> addPropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> editPropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> removePropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemWindowViewModel(EditItemWindow window, ItemData item, bool isNewItem)
        {
            window.Title = $"{item.debugName} [ID {item.id}]";
            _window = window;
            _isNewItem = isNewItem;

            editableItem = item.Clone();
            itemTypeList = EnumValueModel<ItemType>.CreateCollection(excludeValue: ItemType.Any);
            itemRarityList = EnumValueModel<ItemRarity>.CreateCollection();
            itemAbilities = new ObservableCollection<ItemAbilityBase>(editableItem.abilities);
            itemProperties = new ObservableCollection<ItemPropertyBase>(editableItem.properties);

            _selectedItemType = EnumValueModel<ItemType>.GetModel(itemTypeList, editableItem.itemType);
            _selectedItemRarity = EnumValueModel<ItemRarity>.GetModel(itemRarityList, editableItem.itemRarity);

            addAbilityCommand = ReactiveCommand.Create(AddNewAbility);
            editAbilityCommand = ReactiveCommand.Create(EditSelectedAbility);
            removeAbilityCommand = ReactiveCommand.Create(RemoveSelectedAbility);
            addPropertyCommand = ReactiveCommand.Create(AddNewProperty);
            editPropertyCommand = ReactiveCommand.Create(EditSelectedProperty);
            removePropertyCommand = ReactiveCommand.Create(RemoveSelectedProperty);
            saveCommand = ReactiveCommand.Create(SaveItem);
            cancelCommand = ReactiveCommand.Create(OnCancelSaving);
        }

        private void AddNewAbility()
        {
            var newAbility = editableItem.AddEmptyAbility();
            itemAbilities.Add(newAbility);
            selectedAbility = newAbility;
            EditSelectedAbility();
        }

        private void EditSelectedAbility()
        {
            var window = new EditItemAbilityWindow();
            window.DataContext = new EditItemAbilityWindowViewModel(window, _selectedAbility, (modifiedAbility) =>
            {
                var index = editableItem.abilities.IndexOf(_selectedAbility);
                editableItem.abilities[index] = modifiedAbility;
                itemAbilities[index] = modifiedAbility;
            });
            window.ShowDialog(_window);
        }

        private void RemoveSelectedAbility()
        {
            var index = editableItem.abilities.IndexOf(_selectedAbility);
            editableItem.RemoveAbility(index);
            itemAbilities.RemoveAt(index);
        }

        private void AddNewProperty()
        {
            var newProperty = editableItem.AddEmptyProperty();
            itemProperties.Add(newProperty);
            selectedProperty = newProperty;
            EditSelectedProperty();
        }

        private void EditSelectedProperty()
        {
            var window = new EditItemPropertyWindow();
            window.DataContext = new EditItemPropertyWindowViewModel(window, _selectedProperty, (modifiedProperty) => 
            {
                var index = editableItem.properties.IndexOf(_selectedProperty);
                editableItem.properties[index] = modifiedProperty;
                itemProperties[index] = modifiedProperty;
            });
            window.ShowDialog(_window);
        }

        private void RemoveSelectedProperty()
        {
            var index = editableItem.properties.IndexOf(_selectedProperty);
            editableItem.RemoveProperty(index);
            itemProperties.RemoveAt(index);
        }

        private void OnCancelSaving()
        {
            if (_isNewItem)
            {
                Scripts.GameCore.GameDataBase.GameDataBase.instance.items.RemoveData(editableItem.id);
            }
            _window.Close();
        }

        private void SaveItem()
        {
            foreach (var property in itemProperties)
            {
                if (property.propertyType == PropertyType.None)
                    return;
            }

            var dataBase = Scripts.GameCore.GameDataBase.GameDataBase.instance.items;
            dataBase.ChangeData(editableItem.id, editableItem);
            _window.Close();
        }

    }
}
