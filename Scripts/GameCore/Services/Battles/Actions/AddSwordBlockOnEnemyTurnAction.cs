using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public class AddSwordBlockOnEnemyTurnAction : IBattleAction
{
    private readonly DamageInfo _resistance;
    private readonly BattleTurn _battleTurn;

    public AddSwordBlockOnEnemyTurnAction(BattleTurn battleTurn, DamageInfo resistance)
    {
        _battleTurn = battleTurn;
        _resistance = resistance;
    }

    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
        stats.AddOrRemoveResistance(_resistance);
        _battleTurn.onBattleTurnEnd += () => stats.AddOrRemoveResistance(-_resistance);
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordSwordBlock + Localization.Get(session, "battle_action_sword_block_header");
    }

    public string GetDescription(GameSession session)
    {
        var swordOwner = _battleTurn.enemy;
        var sb = new StringBuilder();
        sb.AppendLine(swordOwner == session.player
            ? Localization.Get(session, "battle_action_sword_block_description_mine_usage")
            : Localization.Get(session, "battle_action_sword_block_description_enemy_usage"));
        sb.Append(_resistance.GetCompactView());
        return sb.ToString();
    }

}
