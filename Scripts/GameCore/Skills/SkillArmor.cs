using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillArmor : ISkill
    {
        public ItemType itemType => ItemType.Armor;
        public ResourceId[] requiredFruits => new ResourceId[]
        {
            ResourceId.FruitMandarin,
            ResourceId.FruitPineapple,
            ResourceId.FruitGrape,
        };

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillArmor;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillArmor = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillArmor += value;
        }

    }
}
