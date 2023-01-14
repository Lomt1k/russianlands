﻿using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Managers.Battles.Actions
{
    public class MobAttackAction : IBattleAction
    {
        private readonly MobAttack _mobAttack;
        private readonly DamageInfo _damageInfo;
        private DamageInfo _resultDamage;

        public MobAttackAction(MobAttack attack)
        {
            _mobAttack = attack;
            _damageInfo = _mobAttack.GetRandomValues();
        }

        public void ApplyActionWithMineStats(UnitStats stats)
        {
            stats.currentMana -= _mobAttack.manaCost;
        }

        public void ApplyActionWithEnemyStats(UnitStats stats)
        {
            _resultDamage = stats.TryDealDamage(_damageInfo);
        }

        public string GetHeader(GameSession session)
        {
            return Localization.Get(session, _mobAttack.localizationKey);
        }

        public string GetDescription(GameSession session)
        {
            var sb = new StringBuilder();
            var totalDamage = _resultDamage.GetTotalValue();
            sb.Append(Localization.Get(session, "battle_action_attack_description", totalDamage));

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
