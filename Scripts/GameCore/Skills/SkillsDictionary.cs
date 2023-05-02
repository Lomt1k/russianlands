using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Items;

namespace MarkOne.Scripts.GameCore.Skills;

public class SkillsDictionary
{
    private readonly Dictionary<ItemType, ISkill> _dictionary;

    public ISkill this[ItemType itemType] => _dictionary[itemType];

    public SkillsDictionary()
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

    public IEnumerable<ItemType> GetAllSkillTypes()
    {
        foreach (var key in _dictionary.Keys)
        {
            yield return key;
        }
    }

    public bool ContainsKey(ItemType itemType)
    {
        return _dictionary.ContainsKey(itemType);
    }

}
