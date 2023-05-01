using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillShield : ISkill
    {
        public ItemType itemType => ItemType.Shield;
        public ResourceId[] requiredFruits => new ResourceId[]
        {
            ResourceId.FruitMandarin,
            ResourceId.FruitStrawberry,
            ResourceId.FruitCherry,
        };

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillShield;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillShield = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillShield += value;
        }

    }
}
