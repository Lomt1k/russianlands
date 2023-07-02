using GameDataEditor.ViewModels.UserControls;
using GameDataEditor.Views.UserControls;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace GameDataEditor.ViewModels.Editor.ArenaSettingsEditor;
public class ArenaSettingsEditorViewModel : ViewModelBase
{
    private static GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private ArenaTownhallSettings? _selectedTownhallSettings;

    public ArenaSettings settings { get; }
    public ObjectPropertiesEditorView rewardsForFoodView { get; set; }
    public ObjectPropertiesEditorView rewardsForTicketView { get; set; }
    public ObservableCollection<ArenaTownhallSettings> townhallSettingsList { get; set; } = new();
    public ArenaTownhallSettings? selectedTownhallSettings
    {
        get => _selectedTownhallSettings;
        set => this.RaiseAndSetIfChanged(ref _selectedTownhallSettings, value);
    }

    public ReactiveCommand<Unit, Unit> addTownhallSettingsCommand { get; }
    public ReactiveCommand<Unit, Unit> removeTownhallSettingsCommand { get; }


    public ArenaSettingsEditorViewModel()
    {
        settings = gameDataHolder.arenaSettings;
        rewardsForFoodView = new ObjectPropertiesEditorView() { DataContext = new ObjectPropertiesEditorViewModel(settings.battleRewardsForFood) };
        rewardsForTicketView = new ObjectPropertiesEditorView() { DataContext = new ObjectPropertiesEditorViewModel(settings.battleRewardsForTicket) };
        RefreshTownhallSettingsCollection();

        addTownhallSettingsCommand = ReactiveCommand.Create(AddTownhallSettings);
        removeTownhallSettingsCommand = ReactiveCommand.Create(RemoveTownhallSettings);
    }

    private void AddTownhallSettings()
    {
        settings.townhallSettings.Add(new ArenaTownhallSettings());
        RefreshTownhallSettingsCollection();
    }

    private void RemoveTownhallSettings()
    {
        if (selectedTownhallSettings is null)
            return;

        settings.townhallSettings.Remove(selectedTownhallSettings);
        RefreshTownhallSettingsCollection();
    }

    private void RefreshTownhallSettingsCollection()
    {
        townhallSettingsList.Clear();
        foreach (var item in settings.townhallSettings)
        {
            townhallSettingsList.Add(item);
        }
    }

}
