using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.GameCore.Services.Battles.Actions;

namespace TextGameRPG.Scripts.GameCore.Units.ActionHandlers
{
    public interface IBattleActionHandler
    {
        public Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn);
        public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo);
        public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo);
        public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn);
    }
}
