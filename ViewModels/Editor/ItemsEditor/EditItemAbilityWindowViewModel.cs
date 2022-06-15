using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using TextGameRPG.Models;
using TextGameRPG.Models.Editor.ItemsEditor;
using TextGameRPG.Scripts.GameCore.Items.ItemAbilities;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemAbilityWindowViewModel : ViewModelBase
    {
        private ItemAbilityBase _tempAbility;
        private ObservableCollection<FieldModel> _abilityFields = new ObservableCollection<FieldModel>();
        private EnumValueModel<AbilityType> _selectedAbilityType;
        private Action<ItemAbilityBase> _onEditEnd;

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

        public ObservableCollection<FieldModel> abilityFields => _abilityFields;

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
            FieldModel.FillObservableCollection(ref _abilityFields, ref _tempAbility);
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
}
