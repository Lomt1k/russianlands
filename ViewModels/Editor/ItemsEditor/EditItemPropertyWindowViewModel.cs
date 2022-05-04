using System;
using TextGameRPG.Views.Editor.ItemsEditor;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using TextGameRPG.Models;
using TextGameRPG.Models.Editor.ItemsEditor;
using System.Linq;
using TextGameRPG.Scripts.GameCore.Items.ItemProperties;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemPropertyWindowViewModel : ViewModelBase
    {
        private ItemPropertyBase _tempProperty;
        private EnumValueModel<ItemPropertyType> _selectedPropertyType;
        private Action<ItemPropertyBase> _onEditEnd;

        public ItemPropertyBase tempProperty
        {
            get => _tempProperty;
            set
            {
                this.RaiseAndSetIfChanged(ref _tempProperty, value);
                RefreshFields();
            }
        }

        public ObservableCollection<EnumValueModel<ItemPropertyType>> propertyTypesList { get; }
        public EnumValueModel<ItemPropertyType> selectedPropertyType
        {
            get => _selectedPropertyType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPropertyType, value);
                tempProperty = ItemPropertyRegistry.GetProperty(value.value).Clone();
            }
        }

        public ObservableCollection<PropertyFieldModel> propertyFields { get; }

        public Action closeWindow { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemPropertyWindowViewModel(EditItemPropertyWindow window, ItemPropertyBase property, Action<ItemPropertyBase> onEditEnd)
        {
            propertyFields = new ObservableCollection<PropertyFieldModel>();
            _tempProperty = tempProperty = property.Clone();

            propertyTypesList = EnumValueModel<ItemPropertyType>.CreateCollection(excludeValue: ItemPropertyType.None);
            _selectedPropertyType = EnumValueModel<ItemPropertyType>.GetModel(propertyTypesList, tempProperty.propertyType);

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
