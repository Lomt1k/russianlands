using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillShield : ISkill
    {
        public ItemType itemType => ItemType.Shield;

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
