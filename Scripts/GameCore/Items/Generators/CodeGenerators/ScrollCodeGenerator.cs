using System;

namespace TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators;

public class ScrollCodeGenerator : ItemCodeGeneratorBase
{
    private readonly string[] _mainOption = new[] { "DF", "DC", "DL" };

    public ScrollCodeGenerator(ItemType _type, Rarity _rarity, int _townHallLevel) : base(_type, _rarity, _townHallLevel)
    {
        if (type != ItemType.Scroll)
        {
            throw new ArgumentException($"{GetType()} can not generate item with type '{_type}'");
        }
    }

    public override void AppendSpecificInfo()
    {
        var random = new Random();
        var index = random.Next(_mainOption.Length);
        sb.Append(_mainOption[index]);

        var mana = random.Next(2, 6);
        sb.Append($"M{mana}");
    }


}
