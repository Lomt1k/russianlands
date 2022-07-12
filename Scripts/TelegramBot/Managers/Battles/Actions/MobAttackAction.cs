using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions
{
    public class MobAttackAction : IBattleAction
    {
        public BattleActionType actionType => BattleActionType.Attack;

        private readonly MobAttack _mobAttack;

        public MobAttackAction(MobAttack attack)
        {
            _mobAttack = attack;
        }

        public void ActionWithMineStats(UnitStats stats)
        {
        }

        public void ActionWithEnemyStats(UnitStats stats)
        {
            var damageInfo = _mobAttack.GetRandomValues();
            stats.TryDealDamage(damageInfo);
        }        

        public string? GetLocalization(GameSession session)
        {
            throw new NotImplementedException();
        }
    }
}
