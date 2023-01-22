using System.Collections.Generic;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    public class SkillsDictionary
    {
        private Dictionary<ItemType, ISkill> _dictionary;

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
                { ItemType.Helmet, new SkillHelmet() },
                { ItemType.Boots, new SkillBoots() },
                { ItemType.Shield, new SkillShield() },
            };
        }

        public static IEnumerable<ItemType> GetAllSkillTypes()
        {
            yield return ItemType.Sword;
            yield return ItemType.Bow;
            yield return ItemType.Stick;
            yield return ItemType.Scroll;
            yield return ItemType.Armor;
            yield return ItemType.Helmet;
            yield return ItemType.Boots;
            yield return ItemType.Shield;
        }

    }
}
