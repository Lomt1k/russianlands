using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public class AddShieldOnEnemyTurnAction : IBattleAction
    {
        public BattleActionPriority priority => BattleActionPriority.BeforeAttack;
        public ActivationType activationType => ActivationType.EveryTurn;

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

        public string GetLocalization(GameSession session)
        {
            var shieldOwner = _battleTurn.enemy;
            string header = string.Empty;
            switch (shieldOwner)
            {
                case Player player:
                    var shield = player.inventory.equipped[ItemType.Shield];
                    if (shield == null)
                        break;

                    header = $"<b>{shield.GetFullName(session)}</b>";
                    break;
                default:
                    header = $"<b>{Emojis.items[ItemType.Shield]} {Localization.Get(session, "battle_shield_default_header")}</b>";
                    break;
            }

            var sb = new StringBuilder();
            sb.AppendLine(header);
            sb.AppendLine(shieldOwner == session.player
                ? Localization.Get(session, "battle_shield_mine_usage")
                : Localization.Get(session, "battle_shield_enemy_usage"));

            sb.Append(_resistance.GetCompactView());
            return sb.ToString();
        }
    }
}
