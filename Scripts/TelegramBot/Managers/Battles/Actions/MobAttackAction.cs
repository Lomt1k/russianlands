using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public class MobAttackAction : IBattleAction
    {
        public BattleActionPriority priority => BattleActionPriority.OnAttack;

        private readonly MobAttack _mobAttack;
        private readonly DamageInfo _damageInfo;
        private DamageInfo _resultDamage;

        public MobAttackAction(MobAttack attack, float gradeMult)
        {
            _mobAttack = attack;
            _damageInfo = _mobAttack.GetRandomValues(gradeMult);
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            _resultDamage = stats.TryDealDamage(_damageInfo);
        }        

        public string GetLocalization(GameSession session)
        {
            var sb = new StringBuilder();
            var totalDamage = _damageInfo.GetTotalValue();
            sb.AppendLine(Localization.Get(session, _mobAttack.localizationKey));
            sb.Append(string.Format(Localization.Get(session, "battle_action_attack_description"), totalDamage));
            var resultDamageView = _resultDamage.GetCompactView();
            if (resultDamageView != null)
            {
                sb.AppendLine();
                sb.Append(resultDamageView);
            }
            return sb.ToString();
        }
    }
}
