using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items;

namespace MarkOne.Scripts.GameCore.Skills;

public static class SkillsDictionary
{
    private static readonly Dictionary<ItemType, ISkill> _dictionary;

    static SkillsDictionary()
    {
        _dictionary = new Dictionary<ItemType, ISkill>
        {
            { ItemType.Sword, new SkillSword() },
            { ItemType.Bow, new SkillBow() },
            { ItemType.Stick, new SkillStick() },
            { ItemType.Scroll, new SkillScroll() },
            { ItemType.Armor, new SkillArmor() },
            { ItemType.Shield, new SkillShield() },
            { ItemType.Helmet, new SkillHelmet() },
            { ItemType.Boots, new SkillBoots() },
        };
    }

    public static ISkill Get(ItemType itemType)
    {
        return _dictionary[itemType];
    }

    public static IEnumerable<ItemType> GetAllSkillTypes()
    {
        foreach (var key in _dictionary.Keys)
        {
            yield return key;
        }
    }

    public static bool ContainsKey(ItemType itemType)
    {
        return _dictionary.ContainsKey(itemType);
    }

}
