using DynamicData;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Items;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace MarkOne.ViewModels.Editor.ArenaLeaguesEdittor;
public class FakePlayerGenerationSettingsViewModel : ViewModelBase
{
    private string _header = string.Empty;
    private FakePlayerGenerationSettings? _settings;
    private WeightedRarity? _selectedItemRarity;

    public string header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }
    public FakePlayerGenerationSettings? settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }
    public WeightedRarity? selectedItemRarity
    {
        get => _selectedItemRarity;
        set => this.RaiseAndSetIfChanged(ref _selectedItemRarity, value);
    }
    public ObservableCollection<WeightedRarity> itemRarities { get; } = new();

    public ReactiveCommand<Unit, Unit> addItemRarityCommand { get; }
    public ReactiveCommand<Unit, Unit> removeItemRarityCommand { get; }

    public FakePlayerGenerationSettingsViewModel()
    {
        addItemRarityCommand = ReactiveCommand.Create(AddItemRarity);
        removeItemRarityCommand = ReactiveCommand.Create(RemoveItemRarity);
    }

    public void SetModel(FakePlayerGenerationSettings _settings, string _header)
    {
        settings = _settings;
        header = _header;
        RefreshItemRaritiesCollection();
    }

    private void AddItemRarity()
    {
        settings?.itemRarities.Add(new WeightedRarity());
        RefreshItemRaritiesCollection();
    }

    private void RemoveItemRarity()
    {
        if (selectedItemRarity is null)
            return;

        settings?.itemRarities.Remove(selectedItemRarity);
        RefreshItemRaritiesCollection();
    }

    private void RefreshItemRaritiesCollection()
    {
        itemRarities.Clear();
        if (settings != null)
        {
            itemRarities.AddRange(settings.itemRarities);
        }
    }
}
