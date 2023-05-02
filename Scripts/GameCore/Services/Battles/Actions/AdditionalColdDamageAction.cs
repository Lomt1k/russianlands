using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

// Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
public class AdditionalColdDamageAction : IBattleAction
{
    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordAdditionalDamage + Localization.Get(session, "battle_action_additional_cold_damage_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_additional_cold_damage_description");
    }


}
