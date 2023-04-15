using ReactiveUI;
using System.Reactive;
using TextGameRPG.Models.UserControls;
using TextGameRPG.Scripts.GameCore.Services;
using TextGameRPG.Scripts.GameCore.Services.GameData;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Views.UserControls;

namespace TextGameRPG.ViewModels.Editor.PotionsEditor
{
    public class PotionInspectorViewModel : ViewModelBase
    {
        private static readonly GameDataHolder gameDataBase = Services.Get<GameDataHolder>();

        private PotionData? _tempPotion;
        private ObjectFieldsEditorView? _potionFieldsEditorView;

        public PotionData? tempPotion
        {
            get => _tempPotion;
            set => this.RaiseAndSetIfChanged(ref _tempPotion, value);
        }
        public ObjectFieldsEditorView? potionFieldsEditorView
        {
            get => _potionFieldsEditorView;
            set => this.RaiseAndSetIfChanged(ref _potionFieldsEditorView, value);
        }

        public ReactiveCommand<Unit, Unit> saveChangesCommand { get; }
        public ReactiveCommand<Unit, Unit> cancelChangesCommand { get; }

        public PotionInspectorViewModel()
        {
            saveChangesCommand = ReactiveCommand.Create(SaveChanges);
            cancelChangesCommand = ReactiveCommand.Create(ResetChanges);
        }

        public void ShowPotion(PotionData potionData)
        {
            tempPotion = potionData.Clone();
            potionFieldsEditorView = UserControlsHelper.CreateObjectEditorView(tempPotion);
        }

        private void SaveChanges()
        {
            if (_tempPotion == null)
                return;

            potionFieldsEditorView.vm.SaveObjectChanges();
            gameDataBase.potions.ChangeData(_tempPotion.id, _tempPotion);
        }

        private void ResetChanges()
        {
            if (tempPotion == null)
                return;

            var id = tempPotion.id;
            var originalData = gameDataBase.potions[id];
            ShowPotion(originalData);
        }

    }
}
