using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillSword : ISkill
    {
        public ItemType itemType => ItemType.Sword;
        public ResourceType[] requiredFruits => new ResourceType[]
        {
            ResourceType.FruitApple,
            ResourceType.FruitPineapple,
            ResourceType.FruitBlueberry,
        };

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillSword;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillSword = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillSword += value;
        }

    }
}
