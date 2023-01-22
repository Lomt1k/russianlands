using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillArmor : ISkill
    {
        public ItemType itemType => ItemType.Armor;

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
