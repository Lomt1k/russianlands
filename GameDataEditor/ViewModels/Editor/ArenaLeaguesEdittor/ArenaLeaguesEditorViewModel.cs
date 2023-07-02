using GameDataEditor.Views.Editor.ArenaLeaguesEditor;
using MarkOne.Models;
using MarkOne.Scripts.GameCore.Arena;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace GameDataEditor.ViewModels.Editor.ArenaLeaguesEdittor;
public class ArenaLeaguesEditorViewModel : ViewModelBase
{
    private EnumValueModel<LeagueId>? _selectedLeague;

    public ObservableCollection<EnumValueModel<LeagueId>> leagues { get; }

    public ArenaLeagueInspectorView leagueInspectorView { get; }
    public ArenaLeagueInspectorViewModel leagueInspectorViewModel { get; }

    public EnumValueModel<LeagueId>? selectedLeague
    {
        get => _selectedLeague;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedLeague, value);
            if (value != null)
            {
                leagueInspectorViewModel.SetModel(value.value);
            }
        }
    }

    public ArenaLeaguesEditorViewModel()
    {
        leagues = EnumValueModel<LeagueId>.CreateCollection();
        leagueInspectorView = new ArenaLeagueInspectorView();
        leagueInspectorView.DataContext = leagueInspectorViewModel = new ArenaLeagueInspectorViewModel();
    }

}
