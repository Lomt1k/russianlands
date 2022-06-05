using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Units
{
    public class PlayerResources
    {
        private GameSession _session;

        public int gold
        {
            get => _session.profile.data.resourceGold;
            set => _session.profile.data.resourceGold = value;
        }
        public int food
        {
            get => _session.profile.data.resourceFood;
            set => _session.profile.data.resourceFood = value;
        }
        public int diamonds
        {
            get => _session.profile.data.resourceDiamonds;
            set => _session.profile.data.resourceDiamonds = value;
        }

        public PlayerResources(GameSession session)
        {
            _session = session;
        }

        public string GetView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localizations.Localization.Get(_session, "unit_view_header_resources"));
            sb.AppendLine($"{Emojis.resources[Resource.Gold]} {gold.View()}" +
                $"{Emojis.bigSpace}{Emojis.resources[Resource.Food]} {food.View()}" +
                $"{Emojis.bigSpace}{Emojis.resources[Resource.Diamond]} {diamonds.View()}");

            return sb.ToString();
        }


    }
}
