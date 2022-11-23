using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.GameCore.Managers.Battles.Actions;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public interface IBattleActionHandler
    {
        public Task<IBattleAction?> GetAttackAction(BattleTurn battleTurn);
        public bool TryAddShieldOnStartEnemyTurn(out DamageInfo damageInfo);
        public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn);
    }
}
