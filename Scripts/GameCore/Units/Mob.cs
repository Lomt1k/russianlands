using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Mobs;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Units.ActionHandlers;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Mob : IBattleUnit
    {
        public GameSession session { get; }
        public UnitStats unitStats { get; }
        public IBattleActionHandler actionHandler { get; }
        public IMobData mobData { get; }

        public string nickname => Localization.Get(session, mobData.localizationKey);

        public Mob(GameSession _session, IMobData _data)
        {
            session = _session;
            mobData = _data;

            unitStats = new MobStats(this);
            actionHandler = new MobActionHandler(this);
        }

        public string GetGeneralUnitInfoView(GameSession sessionToSend)
        {
            var sb = new StringBuilder();
            sb.AppendLine(nickname.Bold());
            string levelStr = Localization.Get(sessionToSend, "unit_view_level", mobData.statsSettings.level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }
        public string GetFullUnitInfoView(GameSession sessionToSend, bool withHealth = true)
        {
            var sb = new StringBuilder();
            sb.Append(GetGeneralUnitInfoView(sessionToSend));

            sb.AppendLine();
            sb.Append(GetAttacksView());

            sb.AppendLine();
            sb.AppendLine(unitStats.GetView(sessionToSend, withHealth));

            return sb.ToString();
        }

        private string GetAttacksView()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < mobData.mobAttacks.Count; i++)
            {
                if (i > 0)
                {
                    sb.AppendLine();
                }
                sb.AppendLine(mobData.mobAttacks[i].GetView(session));
            }
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
