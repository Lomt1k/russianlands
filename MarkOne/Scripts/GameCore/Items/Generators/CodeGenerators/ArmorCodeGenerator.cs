﻿using System;
using System.Collections.Generic;

namespace MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;

/// <summary>
/// For armor, boots and helmet
/// </summary>
public class ArmorCodeGenerator : ItemCodeGeneratorBase
{
    private List<Func<bool>> _options => new List<Func<bool>>
    {
        () => { sb.Append("DF"); return true; }, //damage resist fire
        () => { sb.Append("DC"); return true; }, //damage resist cold
        () => { sb.Append("DL"); return true; }, //damage resist lightning
    };

    public ArmorCodeGenerator(ItemType _type, Rarity _rarity, byte _townHallLevel, byte? grade = null) : base(_type, _rarity, _townHallLevel, grade)
    {
        if (type != ItemType.Armor && type != ItemType.Helmet && type != ItemType.Boots)
        {
            throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
        }
    }

    public override void AppendSpecificInfo()
    {
        var random = new Random();
        var needOptionsCount = 0;
        switch (rarity)
        {
            case Rarity.Rare: needOptionsCount = 1; break;
            case Rarity.Epic: needOptionsCount = 2; break;
            case Rarity.Legendary: needOptionsCount = 3; break;
        }

        while (needOptionsCount > 0)
        {
            var index = random.Next(_options.Count);
            var result = _options[index]();
            if (result)
            {
                needOptionsCount--;
            }
        }
    }


}
