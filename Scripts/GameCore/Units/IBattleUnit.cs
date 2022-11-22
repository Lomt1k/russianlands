using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public interface IBattleUnit
    {
        public string nickname { get; }
        public UnitStats unitStats { get; }
        public GameSession session { get; }

        public string GetGeneralUnitInfoView(GameSession sessionToSend);
        public string GetFullUnitInfoView(GameSession sessionToSend);

        public Task OnStartBattle(Battle battle);
        public Task<IBattleAction?> GetAttackActionForBattleTurn(BattleTurn battleTurn);
        public Task OnStartEnemyTurn(BattleTurn battleTurn);
        public bool TryAddShieldOnStartEnemyTurn(out DamageInfo damageInfo);
        public void OnMineBattleTurnAlmostEnd();
        public Task OnMineBatteTurnTimeEnd();
        public Task OnEnemyBattleTurnTimeEnd();
    }
}
