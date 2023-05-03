using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;

namespace MarkOne.Scripts.GameCore.Services.Battles;

public class BattleManager : Service
{
    private readonly List<Battle> _battles = new List<Battle>();
    private readonly Dictionary<Player, Battle> _battlesByPlayers = new Dictionary<Player, Battle>();

    public BattleManager()
    {
    }

    public Battle StartBattlePVP(Player opponentA, Player opponentB, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        var battle = new Battle(opponentA, opponentB, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc);
        RegisterBattle(battle);
        battle.StartBattle();
        return battle;
    }

    public Battle StartBattleWithMob(Player player, IMobData mobData, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        var mob = new Mob(player.session, mobData);
        return StartBattleWithMob(player, mob, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc);
    }

    public Battle StartBattleWithMob(Player player, Mob mob, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        var battle = new Battle(player, mob, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc);
        RegisterBattle(battle);
        battle.StartBattle();
        return battle;
    }

    private void RegisterBattle(Battle battle)
    {
        _battles.Add(battle);
        if (battle.firstUnit is Player firstPlayer)
        {
            _battlesByPlayers[firstPlayer] = battle;
        }
        if (battle.secondUnit is Player secondPlayer)
        {
            _battlesByPlayers[secondPlayer] = battle;
        }
    }

    public Battle? GetCurrentBattle(Player player)
    {
        _battlesByPlayers.TryGetValue(player, out var battle);
        return battle;
    }

    public void UnregisterBattle(Battle battle)
    {
        _battles.Remove(battle);
        if (battle.firstUnit is Player firstPlayer)
        {
            _battlesByPlayers.Remove(firstPlayer);
        }
        if (battle.secondUnit is Player secondPlayer)
        {
            _battlesByPlayers.Remove(secondPlayer);
        }
    }

    public void UnregisterAllBattles()
    {
        _battles.Clear();
        _battlesByPlayers.Clear();
    }

    public async void OnSessionClosedWithError(Player player)
    {
        var battle = GetCurrentBattle(player);
        if (battle != null)
        {
            await battle.ForceBattleEndWithResult(player, BattleResult.Lose).FastAwait();
        }
    }

    public List<Player> GetAllPlayers()
    {
        return _battlesByPlayers.Keys.ToList();
    }

}
