using System.Threading.Tasks;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Units.ActionHandlers;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Units;

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
