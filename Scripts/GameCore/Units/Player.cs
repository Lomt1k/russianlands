﻿using System.Text;
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
        public PlayerResources resources { get; private set; }
        public PlayerInventory inventory => session.profile.dynamicData.inventory;
        public string nickname => session.profile.data.nickname;

        public Player(GameSession _session)
        {
            session = _session;
            unitStats = new PlayerStats(this);
            resources = new PlayerResources(_session);
        }

        public string GetUnitView()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{nickname}{Emojis.menuItems[MenuItem.Character]}</b>");
            string levelStr = string.Format(Localizations.Localization.Get(session, "unit_view_level"), session.profile.data.level);
            sb.AppendLine(levelStr);

            sb.AppendLine(unitStats.GetView());
            return sb.ToString();
        }
    }
}
