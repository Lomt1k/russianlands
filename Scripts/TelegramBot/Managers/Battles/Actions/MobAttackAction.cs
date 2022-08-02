using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Items;
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
            stats.TryDealDamage(_damageInfo);
        }        

        public string? GetLocalization(GameSession session)
        {
            return "TODO: MobAttackAction localization";
        }
    }
}
