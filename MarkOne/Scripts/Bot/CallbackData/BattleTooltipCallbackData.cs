﻿using Newtonsoft.Json;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using FastTelegramBot.DataTypes.Keyboards;

namespace MarkOne.Scripts.Bot.CallbackData;

public enum BattleTooltipType : byte
{
    ShowMineStats = 0,
    ShowEnemyStats = 1
}

public static class BattleTooltipTypeExtensions
{
    /// <returns>Должен ли игнорироваться повторный вызов данного тултипа в течении одного хода</returns>
    public static bool IsSecondQueryIgnoreRequired(this BattleTooltipType type)
    {
        return type == BattleTooltipType.ShowMineStats || type == BattleTooltipType.ShowEnemyStats;
    }
}

public class BattleTooltipCallbackData : CallbackDataBase
{
    public BattleTooltipType tooltip { get; set; }
}

public static class BattleToolipHelper
{
    public static InlineKeyboardButton CreateTooltipButton(GameSession session, BattleTooltipType type, string localizationKey)
    {
        var callbackData = new BattleTooltipCallbackData() { tooltip = type };
        var text = Emojis.ElementInfo + Localization.Get(session, localizationKey);
        return InlineKeyboardButton.WithCallbackData(text, JsonConvert.SerializeObject(callbackData));
    }

    public static InlineKeyboardMarkup GetStatsKeyboard(GameSession session)
    {
        var mineStatsButton = CreateTooltipButton(session, BattleTooltipType.ShowMineStats, "battle_tooltip_mine_stats");
        var enemyStatsButton = CreateTooltipButton(session, BattleTooltipType.ShowEnemyStats, "battle_tooltip_enemy_stats");
        var buttons = new List<InlineKeyboardButton> { mineStatsButton, enemyStatsButton };
        return new InlineKeyboardMarkup(buttons);
    }
}
