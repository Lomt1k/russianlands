using System.Text;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public class AddShieldOnEnemyTurnAction : IBattleAction
{
    private readonly DamageInfo _resistance;
    private readonly BattleTurn _battleTurn;

    public AddShieldOnEnemyTurnAction(BattleTurn battleTurn, DamageInfo resistance)
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
        var shieldOwner = _battleTurn.enemy;
        var header = string.Empty;
        switch (shieldOwner)
        {
            case Player player:
                var playerShield = player.inventory.equipped[ItemType.Shield];
                if (playerShield == null)
                    break;

                header = playerShield.GetFullName(session).Bold();
                break;
            case FakePlayer fakePlayer:
                var fakePlayerShield = fakePlayer.equippedItems[ItemType.Shield];
                if (fakePlayerShield == null)
                    break;

                header = fakePlayerShield.GetFullName(session).Bold();
                break;
            default:
                header = ItemType.Shield.GetEmoji() + Localization.Get(session, "battle_shield_default_header").Bold();
                break;
        }

        return header;
    }

    public string GetDescription(GameSession session)
    {
        var shieldOwner = _battleTurn.enemy;
        var sb = new StringBuilder();
        sb.AppendLine(shieldOwner == session.player
            ? Localization.Get(session, "battle_shield_mine_usage")
            : Localization.Get(session, "battle_shield_enemy_usage"));
        sb.Append(_resistance.GetCompactView());
        return sb.ToString();
    }

}
