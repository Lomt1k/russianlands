using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using System.Linq;
using TextGameRPG.Views.Editor.PotionsEditor;
using System.Reactive;
using TextGameRPG.Scripts.GameCore.GameDataBase;
using TextGameRPG.Scripts.GameCore.Potions;
using TextGameRPG.Models.RegularDialogs;
using System;

namespace TextGameRPG.ViewModels.Editor.PotionsEditor
{
    public class PotionsEditorViewModel : ViewModelBase
    {
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

        public PotionsEditorViewModel()
        {
            potionInspector = new PotionInspectorView();
            potionInspector.DataContext = potionInspectorViewModel = new PotionInspectorViewModel();

            addNewPotionCommand = ReactiveCommand.Create(AddNewItem);

            RefreshShowedItems();
            GameDataBase.instance.items.onDataChanged += OnDataBaseChanged;
        }

        private void OnDataBaseChanged()
        {
            RefreshShowedItems();
        }

        private void RefreshShowedItems()
        {
            var potions = GameDataBase.instance.potions.GetAllData();
            RefreshShowedItems(potions);
        }

        private void RefreshShowedItems(IEnumerable<PotionData> potions)
        {
            showedPotions.Clear();
            foreach (var potion in potions)
            {
                showedPotions.Add(potion);
            }
        }

        private void AddNewItem()
        {
            var allPotions = GameDataBase.instance.potions.GetAllData().ToList();
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
            GameDataBase.instance.potions.AddData(newPotionData.id, newPotionData);
            RefreshShowedItems();
            selectedPotion = newPotionData;

            //var inspectorViewModel = potionInspector.DataContext as PotionInspectorViewModel;
            //inspectorViewModel.StartEditItem(isNewItem: true);
        }

    }
}
