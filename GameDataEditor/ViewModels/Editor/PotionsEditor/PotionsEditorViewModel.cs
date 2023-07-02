using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using MarkOne.Models.RegularDialogs;
using MarkOne.Models.UserControls;
using MarkOne.Scripts.GameCore.Potions;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using MarkOne.Views.UserControls;

namespace GameDataEditor.ViewModels.Editor.PotionsEditor;

public class PotionsEditorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataBase = ServiceLocator.Get<GameDataHolder>();


    private PotionData? _selectedPotion;
    private ObjectPropertiesEditorView? _potionPropertiesEditorView;

    public ObservableCollection<PotionData> showedPotions { get; private set; } = new ObservableCollection<PotionData>();

    public PotionData? selectedPotion
    {
        get => _selectedPotion;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPotion, value);
            potionPropertiesEditorView = _selectedPotion != null ? UserControlsHelper.CreateObjectEditorView(_selectedPotion) : null;
        }
    }

    public ObjectPropertiesEditorView? potionPropertiesEditorView
    {
        get => _potionPropertiesEditorView;
        set => this.RaiseAndSetIfChanged(ref _potionPropertiesEditorView, value);
    }

    public ReactiveCommand<Unit, Unit> addNewPotionCommand { get; }
    public ReactiveCommand<Unit, Unit> removePotionCommand { get; }

    public PotionsEditorViewModel()
    {
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
        var newPotionId = allPotions.Count > 0 ? allPotions.Max(x => x.id) + 1 : 1;

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
