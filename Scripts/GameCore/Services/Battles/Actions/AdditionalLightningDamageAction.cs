using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Units.Stats;

namespace TextGameRPG.Scripts.GameCore.Services.Battles.Actions;

// Используется только для вывода локализации, сам урон учитывается в ItemKeywordActionsHandler.cs
public class AdditionalLightingDamageAction : IBattleAction
{
    public void ApplyActionWithMineStats(UnitStats stats)
    {
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
    }

    public string GetHeader(GameSession session)
    {
        return Emojis.StatKeywordAdditionalDamage + Localization.Get(session, "battle_action_additional_lightning_damage_header");
    }

    public string GetDescription(GameSession session)
    {
        return Localization.Get(session, "battle_action_additional_lightning_damage_description");
    }


}
