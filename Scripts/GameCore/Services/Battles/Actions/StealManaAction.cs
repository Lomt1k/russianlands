using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public class StealManaAction : IBattleAction
{
    public void ApplyActionWithMineStats(UnitStats stats)
    {
        stats.currentMana++;
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
        stats.currentMana--;
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordStealMana + Localization.Get(session, "battle_action_mana_steal_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_mana_steal_description");
    }

}
