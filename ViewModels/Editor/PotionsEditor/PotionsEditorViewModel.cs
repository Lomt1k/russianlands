using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;
using TextGameRPG.Views.Editor.PotionsEditor;
using System.Reactive;
using TextGameRPG.Scripts.GameCore.Services.GameDataBase;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Models.RegularDialogs;
using System;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.ViewModels.Editor.PotionsEditor
{
    public class PotionsEditorViewModel : ViewModelBase
    {
        private static readonly GameDataBase gameDataBase = Services.Get<GameDataBase>();


        private PotionData? _selectedPotion;
        public ObservableCollection<PotionData> showedPotions { get; private set; } = new ObservableCollection<PotionData>();

        public PotionData? selectedPotion
        {
            get => _selectedPotion;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedPotion, value);
                if (_selectedPotion != null)
                {
                    potionInspectorViewModel.ShowPotion(_selectedPotion);
                }
            }
        }
        public PotionInspectorView potionInspector { get; }
        public PotionInspectorViewModel potionInspectorViewModel { get; }

        public ReactiveCommand<Unit, Unit> addNewPotionCommand { get; }
        public ReactiveCommand<Unit, Unit> removePotionCommand { get; }

        public PotionsEditorViewModel()
        {
            potionInspector = new PotionInspectorView();
            potionInspector.DataContext = potionInspectorViewModel = new PotionInspectorViewModel();

            addNewPotionCommand = ReactiveCommand.Create(AddNewPotion);
            removePotionCommand = ReactiveCommand.Create(RemoveSelectedPotion);

            RefreshShowedItems();
            gameDataBase.potions.onDataChanged += OnDataBaseChanged;
        }

        private void OnDataBaseChanged()
        {
            RefreshShowedItems();
        }

        private void RefreshShowedItems()
        {
            var potions = gameDataBase.potions.GetAllData();
            RefreshShowedItems(potions);
        }

        private void RefreshShowedItems(IEnumerable<PotionData> potions)
        {
            var oldSelectedId = selectedPotion?.id;
            showedPotions.Clear();

            foreach (var potion in potions)
            {
                showedPotions.Add(potion);
                if (potion.id == oldSelectedId)
                {
                    selectedPotion = potion;
                }
            }
        }

        private void AddNewPotion()
        {
            var allPotions = gameDataBase.potions.GetAllData().ToList();
            int newPotionId = allPotions.Count > 0 ? allPotions.Max(x => x.id) + 1 : 1;

            RegularDialogHelper.ShowItemSelectionDialog("Select potion type", new Dictionary<string, Action>()
            {
                { "Health Restore", () => AddNewPotionData(new HealthRestorePotionData(newPotionId)) },
                { "Add Damage", () => AddNewPotionData(new AddDamagePotionData(newPotionId)) },
                { "Add Resistance", () => AddNewPotionData(new AddResistancePotionData(newPotionId)) },
            });
        }

        private void AddNewPotionData(PotionData newPotionData)
        {
            gameDataBase.potions.AddData(newPotionData.id, newPotionData);
            RefreshShowedItems();
            selectedPotion = newPotionData;
        }

        private void RemoveSelectedPotion()
        {
            if (selectedPotion == null)
                return;

            gameDataBase.potions.RemoveData(selectedPotion.id);
        }

    }
}
