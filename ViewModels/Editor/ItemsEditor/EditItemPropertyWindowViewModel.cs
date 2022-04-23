using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators.ItemPropertyGenerators;
using TextGameRPG.Views.Editor.ItemsEditor;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using TextGameRPG.Models;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemPropertyWindowViewModel : ViewModelBase
    {
        private EnumValueModel<ItemPropertyGeneratorType> _selectedPropertyType;

        public ItemPropertyGeneratorBase tempProperty { get; private set; }

        public ObservableCollection<EnumValueModel<ItemPropertyGeneratorType>> propertyTypesList { get; }
        public EnumValueModel<ItemPropertyGeneratorType> selectedPropertyType
        {
            get => _selectedPropertyType;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPropertyType, value);
                tempProperty = ItemPropertyGeneratorRegistry.GetProperty(value.value).Clone();
                //TODO recreate fields?
            }
        }

        public Action closeWindow { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemPropertyWindowViewModel(EditItemPropertyWindow window, ItemPropertyGeneratorBase property, Action<ItemPropertyGeneratorBase> onEditEnd)
        {
            tempProperty = property.Clone();

            propertyTypesList = EnumValueModel<ItemPropertyGeneratorType>.CreateCollection(excludeValue: ItemPropertyGeneratorType.None);
            _selectedPropertyType = EnumValueModel<ItemPropertyGeneratorType>.GetModel(propertyTypesList, tempProperty.propertyType);

            closeWindow = () => window.Close();
            saveCommand = ReactiveCommand.Create(() => 
            {
                onEditEnd(tempProperty);
                closeWindow(); 
            });
            cancelCommand = ReactiveCommand.Create(closeWindow);
        }


    }
}
