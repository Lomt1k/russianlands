using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

// Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
public class FinishingAction : IBattleAction
{
    private readonly byte _damageBonusPercentage;

    public FinishingAction(byte damageBonusPercentage)
    {
        _damageBonusPercentage = damageBonusPercentage;
    }

    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordFinishing + Localization.Get(session, "battle_action_finishing_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_finishing_description", _damageBonusPercentage);
    }

}
