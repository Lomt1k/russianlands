using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions;

public class AbsorptionAction : IBattleAction
{
    private readonly int _healthToRestore;

    public AbsorptionAction(int healthToRestore)
    {
        _healthToRestore = healthToRestore;
    }

    public void ApplyActionWithMineStats(UnitStats stats)
    {
        stats.RestoreHealth(_healthToRestore);
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordAbsorption + Localization.Get(session, "battle_action_absorption_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_absorption_description")
            + $"\n{Emojis.StatHealth} {_healthToRestore}";
    }

}
