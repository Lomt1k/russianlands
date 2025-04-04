﻿using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Units.Mobs;
using MarkOne.Scripts.GameCore.Units.Stats;

namespace MarkOne.Scripts.GameCore.Services.Battles.Actions;

public class MobAttackAction : IBattleAction
{
    private readonly MobAttack _mobAttack;
    private readonly DamageInfo _damageInfo;
    private DamageInfo _resultDamage;

    public MobAttackAction(MobAttack attack)
    {
        _mobAttack = attack;
        _damageInfo = _mobAttack.GetRandomValues();
    }

    public void ApplyActionWithMineStats(UnitStats stats)
    {
        stats.currentMana -= _mobAttack.manaCost;
    }

    public void ApplyActionWithEnemyStats(UnitStats stats)
    {
        _resultDamage = stats.TryDealDamage(_damageInfo);
    }

    public string GetHeader(GameSession session)
    {
        var header = Localization.Get(session, _mobAttack.localizationKey)
            + (_mobAttack.manaCost > 0 ? $" {Emojis.StatMana}{_mobAttack.manaCost}" : string.Empty);
        return header.Bold();
    }

    public string GetDescription(GameSession session)
    {
        var sb = new StringBuilder();
        var totalDamage = _resultDamage.GetTotalValue();
        sb.Append(Localization.Get(session, "battle_action_attack_description", totalDamage));

        var resultDamageView = _resultDamage.GetCompactView();
        if (resultDamageView != null)
        {
            sb.AppendLine();
            sb.Append(resultDamageView);
        }
        return sb.ToString();
    }

}
