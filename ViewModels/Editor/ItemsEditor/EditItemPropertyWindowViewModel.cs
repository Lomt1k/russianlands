using System;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators;
using TextGameRPG.Views.Editor.ItemsEditor;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using TextGameRPG.Models;
using TextGameRPG.Models.Editor.ItemsEditor;
using System.Linq;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemPropertyWindowViewModel : ViewModelBase
    {
        private ItemPropertyGeneratorBase _tempProperty;
        private EnumValueModel<ItemPropertyGeneratorType> _selectedPropertyType;
        private Action<ItemPropertyGeneratorBase> _onEditEnd;

        public ItemPropertyGeneratorBase tempProperty
        {
            get => _tempProperty;
            set
            {
                this.RaiseAndSetIfChanged(ref _tempProperty, value);
                RefreshFields();
            }
        }

        public ObservableCollection<EnumValueModel<ItemPropertyGeneratorType>> propertyTypesList { get; }
        public EnumValueModel<ItemPropertyGeneratorType> selectedPropertyType
        {
            get => _selectedPropertyType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPropertyType, value);
                tempProperty = ItemPropertyGeneratorRegistry.GetProperty(value.value).Clone();
            }
        }

        public ObservableCollection<PropertyFieldModel> propertyFields { get; }

        public Action closeWindow { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemPropertyWindowViewModel(EditItemPropertyWindow window, ItemPropertyGeneratorBase property, Action<ItemPropertyGeneratorBase> onEditEnd)
        {
            propertyFields = new ObservableCollection<PropertyFieldModel>();
            _tempProperty = tempProperty = property.Clone();

            propertyTypesList = EnumValueModel<ItemPropertyGeneratorType>.CreateCollection(excludeValue: ItemPropertyGeneratorType.None);
            _selectedPropertyType = EnumValueModel<ItemPropertyGeneratorType>.GetModel(propertyTypesList, tempProperty.propertyType);

            _onEditEnd = onEditEnd;
            closeWindow = () => window.Close();
            saveCommand = ReactiveCommand.Create(SaveProperty);
            cancelCommand = ReactiveCommand.Create(closeWindow);
        }

        private void RefreshFields()
        {
            propertyFields.Clear();
            var fields = _tempProperty.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                var value = fieldInfo.GetValue(_tempProperty);
                var fieldModel = new PropertyFieldModel(fieldInfo, value);
                propertyFields.Add(fieldModel);
            }
        }

        private void SaveProperty()
        {
            foreach (var propertyField in propertyFields)
            {
                if (!propertyField.isValidValue)
                    return;
            }
            UpdateFieldValuesFromModel();

            _onEditEnd(tempProperty);
            closeWindow();
        }

        private void UpdateFieldValuesFromModel()
        {
            var fields = _tempProperty.GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                var fieldModel = propertyFields.Where(x => x.name.Equals(fieldInfo.Name)).First();
                fieldInfo.SetValue(_tempProperty, fieldModel.value);
            }
        }


    }
}
