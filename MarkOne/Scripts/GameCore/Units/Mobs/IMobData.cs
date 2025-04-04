﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Items;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Units.Mobs;

public interface IMobData
{
    string localizationKey { get; set; }
    MobStatsSettings statsSettings { get; }
    List<MobAttack> mobAttacks { get; }
}

[JsonObject]
public class MobStatsSettings
{
    public int level { get; set; } = 1;
    public int health { get; set; } = 100;
    public int physicalResist { get; set; }
    public int fireResist { get; set; }
    public int coldResist { get; set; }
    public int lightningResist { get; set; }
}

[JsonObject]
public class MobAttack
{
    public string localizationKey { get; set; } = "battle_action_mob_normal_attack";
    public sbyte manaCost { get; set; }
    public int minPhysicalDamage { get; set; }
    public int maxPhysicalDamage { get; set; }
    public int minFireDamage { get; set; }
    public int maxFireDamage { get; set; }
    public int minColdDamage { get; set; }
    public int maxColdDamage { get; set; }
    public int minLightningDamage { get; set; }
    public int maxLightningDamage { get; set; }

    public DamageInfo GetRandomValues(float gradeMult = 1f)
    {
        var random = new Random();
        var physicalDamage = random.Next(minPhysicalDamage, maxPhysicalDamage + 1);
        var fireDamage = random.Next(minFireDamage, maxFireDamage + 1);
        var coldDamage = random.Next(minColdDamage, maxColdDamage + 1);
        var lightningDamage = random.Next(minLightningDamage, maxLightningDamage + 1);

        return new DamageInfo(
            (int)Math.Round(physicalDamage * gradeMult),
            (int)Math.Round(fireDamage * gradeMult),
            (int)Math.Round(coldDamage * gradeMult),
            (int)Math.Round(lightningDamage * gradeMult));
    }

    public string GetView(GameSession session)
    {
        var sb = new StringBuilder();
        var header = Localization.Get(session, localizationKey) + ':';
        sb.Append(header.CodeBlock());
        if (maxPhysicalDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatPhysicalDamage + $"{minPhysicalDamage} - {maxPhysicalDamage}");
        }
        if (maxFireDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatFireDamage + $"{minFireDamage} - {maxFireDamage}");
        }
        if (maxColdDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatColdDamage + $"{minColdDamage} - {maxColdDamage}");
        }
        if (maxLightningDamage > 0)
        {
            sb.AppendLine();
            sb.Append(Emojis.StatLightningDamage + $"{minLightningDamage} - {maxLightningDamage}");
        }
        return sb.ToString();
    }

    public MobAttack CloneForMobBuilder()
    {
        var localization = manaCost == 0
            ? "battle_action_mob_normal_attack"
            : "battle_action_mob_strong_attack";
        return new MobAttack()
        {
            localizationKey = localization,
            manaCost = manaCost,
            minPhysicalDamage = minPhysicalDamage,
            maxPhysicalDamage = maxPhysicalDamage,
            minFireDamage = minFireDamage,
            maxFireDamage = maxFireDamage,
            minColdDamage = minColdDamage,
            maxColdDamage = maxColdDamage,
            minLightningDamage = minLightningDamage,
            maxLightningDamage = maxLightningDamage,
        };
    }
}
