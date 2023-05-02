using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;

namespace MarkOne.Scripts.GameCore.Units.ActionHandlers;

public interface IBattleActionHandler
{
    public Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn);
    public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo);
    public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo);
    public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn);
}
