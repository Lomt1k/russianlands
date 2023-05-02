using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public class StunAction : IBattleAction
{
    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
        stats.isSkipNextTurnRequired = true;
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordStun + Localization.Get(session, "battle_action_stun_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_stun_description");
    }
}
