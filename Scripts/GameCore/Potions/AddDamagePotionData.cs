﻿using Newtonsoft.Json;
using System.Text;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Localizations;
using TextGameRPG.Scripts.GameCore.Services.Battles;
using TextGameRPG.Scripts.GameCore.Units;
using TextGameRPG.Scripts.GameCore.Units.Stats.StatEffects;

namespace TextGameRPG.Scripts.GameCore.Potions;

[JsonObject]
public class AddDamagePotionData : PotionData
{
    public int physicalDamage { get; set; }
    public int fireDamage { get; set; }
    public int coldDamage { get; set; }
    public int lightningDamage { get; set; }

    public AddDamagePotionData(int _id) : base(_id)
    {
    }

    public DamageInfo GetValues(GameSession session)
    {
        //TODO: Учитывать атакующие навыки игрока?
        return new DamageInfo(physicalDamage, fireDamage, coldDamage, lightningDamage);
    }

    public override string GetDescription(GameSession sessionForValues, GameSession sessionForView)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Localization.Get(sessionForView, "potion_add_damage_description"));
        sb.AppendLine();
        sb.AppendLine(Localization.Get(sessionForView, "potion_description_damage_header"));
        sb.Append(GetValues(sessionForValues).GetCompactView());
        return sb.ToString();
    }

    public override void Apply(BattleTurn battleTurn, IBattleUnit unit)
    {
        var statEffect = new ExtraDamageStatEffect(GetValues(unit.session));
        unit.unitStats.statEffects.Add(statEffect);
    }
}
