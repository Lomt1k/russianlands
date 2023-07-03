using GameDataEditor.Views.Editor.ArenaLeaguesEditor;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using ReactiveUI;

namespace GameDataEditor.ViewModels.Editor.ArenaLeaguesEdittor;
public class ArenaLeagueInspectorViewModel : ViewModelBase
{
    private static readonly GameDataHolder gameDataHolder = ServiceLocator.Get<GameDataHolder>();

    private ArenaLeagueSettings? _league;

    public ArenaLeagueSettings? league
    {
        get => _league;
        set => this.RaiseAndSetIfChanged(ref _league, value);
    }
    public FakePlayerGenerationSettingsView defaultPlayerView { get; } = new();
    public FakePlayerGenerationSettingsView weakPlayerView { get; } = new();

    public void SetModel(LeagueId leagueId)
    {
        if (!gameDataHolder.arenaLeagueSettings.ContainsKey(leagueId))
        {
            gameDataHolder.arenaLeagueSettings.AddData(leagueId, new ArenaLeagueSettings() { id = leagueId });
        }
        league = gameDataHolder.arenaLeagueSettings[leagueId];
        defaultPlayerView.vm.SetModel(league.defaultPlayerGenerationSettings, "Default Player");
        weakPlayerView.vm.SetModel(league.weakPlayerGenerationSettings, "Weak Player");
    }
}
