using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.Battles.Actions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Units.ActionHandlers;
public class FakePlayerActionHandler : IBattleActionHandler
{
    public FakePlayer fakePlayer { get; }

    public FakePlayerActionHandler(FakePlayer _fakePlayer)
    {
        fakePlayer = _fakePlayer;
    }

    public async Task<List<IBattleAction>> GetActionsBySelectedItem(BattleTurn battleTurn)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IBattleAction> GetEveryTurnActions(BattleTurn battleTurn)
    {
        throw new NotImplementedException();
    }

    public bool TryAddShieldOnEnemyTurn(out DamageInfo damageInfo)
    {
        throw new NotImplementedException();
    }

    public bool TryAddSwordBlockOnEnemyTurn(out DamageInfo damageInfo)
    {
        throw new NotImplementedException();
    }
}
