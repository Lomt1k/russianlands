using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using MarkOne.Models;
using MarkOne.Models.Editor.ItemsEditor;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Views.Editor.ItemsEditor;

namespace GameDataEditor.ViewModels.Editor.ItemsEditor;

public class EditItemAbilityWindowViewModel : ViewModelBase
{
    private ItemAbilityBase _tempAbility;
    private ObservableCollection<ObjectPropertyModel> _abilityFields = new ObservableCollection<ObjectPropertyModel>();
    private EnumValueModel<AbilityType> _selectedAbilityType;
    private readonly Action<ItemAbilityBase> _onEditEnd;

    public ItemAbilityBase tempAbility
    {
        get => _tempAbility;
        set
        {
            this.RaiseAndSetIfChanged(ref _tempAbility, value);
            RefreshFields();
        }
    }

    public ObservableCollection<EnumValueModel<AbilityType>> abilityTypesList { get; }
    public EnumValueModel<AbilityType> selectedAbilityType
    {
        get => _selectedAbilityType;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAbilityType, value);
            tempAbility = ItemAbilityRegistry.GetNewAbility(value.value);
        }
    }

    public ObservableCollection<ObjectPropertyModel> abilityFields => _abilityFields;

    public Action closeWindow { get; }
    public ReactiveCommand<Unit, Unit> saveCommand { get; }
    public ReactiveCommand<Unit, Unit> cancelCommand { get; }

    public EditItemAbilityWindowViewModel(EditItemAbilityWindow window, ItemAbilityBase ability, Action<ItemAbilityBase> onEditEnd)
    {
        _tempAbility = tempAbility = ability.Clone();

        abilityTypesList = EnumValueModel<AbilityType>.CreateCollection(excludeValue: AbilityType.None);
        _selectedAbilityType = EnumValueModel<AbilityType>.GetModel(abilityTypesList, tempAbility.abilityType);

        _onEditEnd = onEditEnd;
        closeWindow = () => window.Close();
        saveCommand = ReactiveCommand.Create(SaveAbility);
        cancelCommand = ReactiveCommand.Create(closeWindow);
    }

    private void RefreshFields()
    {
        abilityFields.Clear();
        ObjectPropertyModel.FillObservableCollection(ref _abilityFields, _tempAbility);
    }

    private void SaveAbility()
    {
        foreach (var field in abilityFields)
        {
            if (!field.isValidValue)
                return;
        }
        UpdateFieldValuesFromModel();

        _onEditEnd(tempAbility);
        closeWindow();
    }

    private void UpdateFieldValuesFromModel()
    {
        var fields = _tempAbility.GetType().GetFields();
        foreach (var fieldInfo in fields)
        {
            var fieldModel = abilityFields.Where(x => x.name.Equals(fieldInfo.Name)).First();
            fieldInfo.SetValue(_tempAbility, fieldModel.value);
        }
    }


}
