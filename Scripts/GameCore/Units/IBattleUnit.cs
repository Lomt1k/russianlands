using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units.ActionHandlers;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public interface IBattleUnit
    {
        public string nickname { get; }
        public UnitStats unitStats { get; }
        public IBattleActionHandler actionHandler { get; }
        public GameSession session { get; }

        public string GetGeneralUnitInfoView(GameSession sessionToSend);
        public string GetFullUnitInfoView(GameSession sessionToSend, bool withHealth = true);

        public Task OnStartBattle(Battle battle);
        public Task OnStartEnemyTurn(BattleTurn battleTurn);
        public void OnMineBattleTurnAlmostEnd();
        public Task OnMineBatteTurnTimeEnd();
        public Task OnEnemyBattleTurnTimeEnd();
    }
}
