using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Arena;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Dialogs.Town.Map.Arena;
public class ArenaDialog : DialogBase
{
    public ArenaDialog(GameSession _session) : base(_session)
    {
    }

    public override Task Start()
    {
        var sb = new StringBuilder()
            .AppendLine(Emojis.ButtonArena + Localization.Get(session, "menu_item_arena").Bold())
            .AppendLine();

        var hasArenaProgress = session.profile.dynamicData.HasArenaProgress();
        if (hasArenaProgress)
        {
            var arenaProgress = session.profile.dynamicData.arenaProgress.EnsureNotNull();
            var targetBattlesCount = gameDataHolder.arenaSettings.battlesInMatch;
            var registrationType = arenaProgress.byTicket
                ? Localization.Get(session, "dialog_arena_registered_by_ticket")
                : Localization.Get(session, "dialog_arena_registered_by_food");
            sb.AppendLine(Localization.Get(session, "dialog_arena_in_progress", registrationType, targetBattlesCount));
            sb.AppendLine();
            sb.AppendLine(GetBattlesProgressView(arenaProgress, targetBattlesCount));
            sb.AppendLine();
            sb.AppendLine(Localization.Get(session, "dialog_arena_battles_to_end", targetBattlesCount - arenaProgress.results.Count));
            // TODO: next battle button
        }
        else
        {
            // TODO
        }
        // TODO
    }

    private string GetBattlesProgressView(PlayerArenaProgress playerArenaProgress, byte battlesCount)
    {
        var sb = new StringBuilder()
            .Append(Localization.Get(session, "dialog_arena_progress_header"));

        var results = playerArenaProgress.results;
        for (int i = 0; i < battlesCount; i++)
        {
            if (i < results.Count)
            {
                var result = results[i];
                var emoji = result.result switch
                {
                    BattleResult.Win => Emojis.ProgressBarPositive,
                    BattleResult.Lose => Emojis.ProgressBarNegative,
                    _ => Emojis.ProgressBarNeutral
                };
                sb.Append(emoji);
            }
            sb.Append(Emojis.ProgressBarEmpty);
        }

        return sb.ToString();
    }

}
