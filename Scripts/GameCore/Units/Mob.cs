using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Managers.Battles;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units.ActionHandlers;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Mob : IBattleUnit
    {
        public GameSession session { get; }
        public UnitStats unitStats { get; }
        public IBattleActionHandler actionHandler { get; }
        public MobData mobData { get; }

        public string nickname => Localization.Get(session, mobData.localizationKey);

        public Mob(GameSession _session, MobData _data)
        {
            session = _session;
            mobData = _data;

            unitStats = new MobStats(this);
            actionHandler = new MobActionHandler(this);
        }

        public string GetGeneralUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}</b>");
            string levelStr = string.Format(Localization.Get(sessionToSend, "unit_view_level"), mobData.statsSettings.level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }
        public string GetFullUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.Append(GetGeneralUnitInfoView(sessionToSend));

            sb.AppendLine();
            sb.AppendLine(unitStats.GetView(sessionToSend));
            return sb.ToString();
        }

        public Task OnStartBattle(Battle battle)
        {
            unitStats.OnStartBattle();
            return Task.CompletedTask;
        }

        public Task OnStartEnemyTurn(BattleTurn battleTurn)
        {
            return Task.CompletedTask;
        }

        public void OnMineBattleTurnAlmostEnd()
        {
            //igored
        }

        public Task OnMineBatteTurnTimeEnd()
        {
            return Task.CompletedTask;
        }

        public Task OnEnemyBattleTurnTimeEnd()
        {
            return Task.CompletedTask;
        }
    }
}
