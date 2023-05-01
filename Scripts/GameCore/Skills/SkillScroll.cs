using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillScroll : ISkill
    {
        public ItemType itemType => ItemType.Scroll;
        public ResourceId[] requiredFruits => new ResourceId[]
        {
            ResourceId.FruitPear,
            ResourceId.FruitStrawberry,
            ResourceId.FruitKiwi,
        };

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillScroll;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillScroll = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillScroll += value;
        }

    }
}
