using System.Text;
using TextGameRPG.Scripts.GameCore.Inventory;
using TextGameRPG.Scripts.GameCore.Units.Stats;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class Player : IBattleUnit
    {
        public GameSession session { get; private set; }
        public UnitStats unitStats { get; private set; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public string nickname => session.profile.data.nickname;

        public Player(GameSession _session)
        {
            session = _session;
            unitStats = new PlayerStats(this);
        }

        public string GetUnitView()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localization.Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);
            sb.AppendLine($"\n{Emojis.atributes[Attribute.Health]} {unitStats.currentHP} / {unitStats.maxHP}" +
                $"{Emojis.space}{Emojis.atributes[Attribute.Mana]} {unitStats.currentMP} / {unitStats.maxMP}");

            sb.AppendLine( Localization.Localization.Get(session, "unit_view_header_resources") );
            sb.AppendLine($"{Emojis.resources[Resource.Gold]} {session.profile.data.resourceGold.View()}" +
                $"{Emojis.space}{Emojis.resources[Resource.Food]} {session.profile.data.resourceFood.View()}" +
                $"{Emojis.space}{Emojis.resources[Resource.Diamond]} {session.profile.data.resourceDiamonds.View()}");

            return sb.ToString();
        }
    }
}
