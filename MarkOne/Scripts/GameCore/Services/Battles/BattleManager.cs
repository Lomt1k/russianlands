using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Dialogs.Battle;
using MarkOne.Scripts.GameCore.Rewards;
using MarkOne.Scripts.GameCore.Units;
using MarkOne.Scripts.GameCore.Units.Mobs;
using MarkOne.Scripts.Utils;

namespace MarkOne.Scripts.GameCore.Services.Battles;

public class BattleManager : Service
{
    private readonly LockedDictionary<Player, Battle> _battlesByPlayers = new();

    public Battle StartBattle(Player player, IMobData mobData, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        var mob = new Mob(player.session, mobData);
        return StartBattle(player, mob, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc);
    }

    public Battle StartBattle(Player opponentA, IBattleUnit opponentB, IEnumerable<RewardBase>? rewards = null,
        Func<Player, BattleResult, Task>? onBattleEndFunc = null,
        Func<Player, BattleResult, Task>? onContinueButtonFunc = null,
        Func<Player, BattleResult, bool>? isAvailableReturnToTownFunc = null)
    {
        var battle = new Battle(opponentA, opponentB, rewards, onBattleEndFunc, onContinueButtonFunc, isAvailableReturnToTownFunc);
        RegisterBattle(battle);
        Task.Run(battle.StartBattle);
        return battle;
    }
    

    private void RegisterBattle(Battle battle)
    {
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
        try
        {
            _battlesByPlayers.TryGetValue(player, out var battle);
            return battle;
        }
        catch (Exception ex)
        {
            Program.logger.Error(ex);
            return null;
        }
    }

    public void UnregisterBattle(Battle battle)
    {
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
        _battlesByPlayers.Clear();
    }

    public List<Player> GetAllPlayers()
    {
        return _battlesByPlayers.Keys.ToList();
    }

}
