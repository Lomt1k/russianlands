using System.Reactive;
using ReactiveUI;
using TextGameRPG.Scripts.GameCore.Items.ItemGenerators;
using TextGameRPG.Views.Editor.ItemsEditor;

namespace TextGameRPG.ViewModels.Editor.ItemsEditor
{
    internal class EditItemWindowViewModel : ViewModelBase
    {
        public ItemGeneratorBase editableItem { get; }

        public ReactiveCommand<Unit, Unit> addPropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> removePropertyCommand { get; }
        public ReactiveCommand<Unit, Unit> saveCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelCommand { get; }

        public EditItemWindowViewModel(EditItemWindow window, ItemGeneratorBase item)
        {
            editableItem = item.Clone();

            addPropertyCommand = ReactiveCommand.Create(AddNewProperty);
            removePropertyCommand = ReactiveCommand.Create(RemoveSelectedProperty);
            saveCommand = ReactiveCommand.Create(SaveItemChanges);
            cancelCommand = ReactiveCommand.Create(() => window.Close());
        }

        private void AddNewProperty()
        {
            //TODO
        }

        private void RemoveSelectedProperty()
        {
            //TODO
        }

        private void SaveItemChanges()
        {
            //TODO
        }

    }
}
