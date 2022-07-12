using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Managers.Battles.Actions;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        public GameSession session { get; private set; }
        public UnitStats unitStats { get; private set; }
        public PlayerResources resources { get; private set; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public string nickname => session.profile.data.nickname;

        public Player(GameSession _session)
        {
            session = _session;
            unitStats = new PlayerStats(this);
            resources = new PlayerResources(_session);
        }

        public string GetGeneralInfoView()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localizations.Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);
            return sb.ToString();
        }

        public string GetFullUnitView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localizations.Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);

            sb.AppendLine(unitStats.GetView(session));
            return sb.ToString();
        }

        public string GetStartTurnView(GameSession session)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");

            sb.AppendLine(unitStats.GetView(session));
            return sb.ToString();
        }

        public Task<List<IBattleAction>> GetActionsForBattleTurn()
        {
            //TODO
            throw new System.NotImplementedException();
        }
    }
}
