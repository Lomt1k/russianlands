using System;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items.ItemAbilities;
using MarkOne.Scripts.GameCore.Items.ItemProperties;

namespace MarkOne.Scripts.GameCore.Items.Generators.CodeGenerators;

public class AmuletCodeGenerator : ItemCodeGeneratorBase
{
    private List<Func<bool>> _options => new List<Func<bool>>
    {
        () => ForceAppendProperty(PropertyType.IncreaseMaxHealth),

        () => { sb.Append("DP"); return true; }, //damage resist physical
        () => { sb.Append("DF"); return true; }, //damage resist fire
        () => { sb.Append("DC"); return true; }, //damage resist cold
        () => { sb.Append("DL"); return true; }, //damage resist lightning

        () => ForceAppendAbility(AbilityType.RestoreHealthEveryTurn),
        () => ForceAppendAbility(AbilityType.AddManaEveryTurn),
    };

    public AmuletCodeGenerator(ItemType _type, Rarity _rarity, byte _townHallLevel, byte? grade = null) : base(_type, _rarity, _townHallLevel, grade)
    {
        if (type != ItemType.Amulet)
        {
            throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
        }
        if (rarity == Rarity.Common)
        {
            throw new ArgumentException($"{GetType()} can not generate items with '{_rarity}' rarity");
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
