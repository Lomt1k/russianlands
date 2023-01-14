using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    public class AddShieldOnEnemyTurnAction : IBattleAction
    {
        private DamageInfo _resistance;
        private BattleTurn _battleTurn;

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
            string header = string.Empty;
            switch (shieldOwner)
            {
                case Player player:
                    var shield = player.inventory.equipped[ItemType.Shield];
                    if (shield == null)
                        break;

                    header = shield.GetFullName(session).Bold();
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
}
