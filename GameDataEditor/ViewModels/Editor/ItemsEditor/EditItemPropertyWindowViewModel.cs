using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using GameDataEditor.Models;
using GameDataEditor.Models.Editor.ItemsEditor;
using MarkOne.Scripts.GameCore.Items.ItemProperties;
using GameDataEditor.Views.Editor.ItemsEditor;

namespace GameDataEditor.ViewModels.Editor.ItemsEditor;

public class EditItemPropertyWindowViewModel : ViewModelBase
{
    private ItemPropertyBase _tempProperty;
    private ObservableCollection<ObjectPropertyModel> _propertyFields = new ObservableCollection<ObjectPropertyModel>();
    private EnumValueModel<PropertyType> _selectedPropertyType;
    private readonly Action<ItemPropertyBase> _onEditEnd;

    public ItemPropertyBase tempProperty
    {
        get => _tempProperty;
        set
        {
            this.RaiseAndSetIfChanged(ref _tempProperty, value);
            RefreshFields();
        }
    }

    public ObservableCollection<EnumValueModel<PropertyType>> propertyTypesList { get; }
    public EnumValueModel<PropertyType> selectedPropertyType
    {
        get => _selectedPropertyType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPropertyType, value);
            tempProperty = ItemPropertyRegistry.GetNewProperty(value.value);
        }
    }

    public ObservableCollection<ObjectPropertyModel> propertyFields => _propertyFields;

    public Action closeWindow { get; }
    public ReactiveCommand<Unit, Unit> saveCommand { get; }
    public ReactiveCommand<Unit, Unit> cancelCommand { get; }

    public EditItemPropertyWindowViewModel(EditItemPropertyWindow window, ItemPropertyBase property, Action<ItemPropertyBase> onEditEnd)
    {
        _tempProperty = tempProperty = property.Clone();

        propertyTypesList = EnumValueModel<PropertyType>.CreateCollection(excludeValue: PropertyType.None);
        _selectedPropertyType = EnumValueModel<PropertyType>.GetModel(propertyTypesList, tempProperty.propertyType);

        _onEditEnd = onEditEnd;
        closeWindow = () => window.Close();
        saveCommand = ReactiveCommand.Create(SaveProperty);
        cancelCommand = ReactiveCommand.Create(closeWindow);
    }

    private void RefreshFields()
    {
        propertyFields.Clear();
        ObjectPropertyModel.FillObservableCollection(ref _propertyFields, _tempProperty);
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
