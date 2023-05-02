using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Stats;
using MarkOne.Scripts.Utils;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public enum SanctionType : byte
{
    RemoveArrow,
    RewoveMana
}

public class SanctionsAction : IBattleAction
{
    private SanctionType _sanctionType;

    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
        _sanctionType = stats.currentArrows < 1 ? SanctionType.RewoveMana
            : Randomizer.TryPercentage(50) ? SanctionType.RemoveArrow : SanctionType.RewoveMana;

        switch (_sanctionType)
        {
            case SanctionType.RewoveMana:
                stats.currentMana--;
                break;
            case SanctionType.RemoveArrow:
                stats.currentArrows--;
                break;
        }
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordSanctions + Localization.Get(session, "battle_action_sanctions_header");
    }

    public string GetDescription(GameSession session)
    {
        return _sanctionType switch
        {
            SanctionType.RewoveMana => Localization.Get(session, "battle_action_sanctions_remove_mana_description"),
            SanctionType.RemoveArrow => Localization.Get(session, "battle_action_sanctions_remove_arrow_description"),
            _ => string.Empty
        };
    }
}
