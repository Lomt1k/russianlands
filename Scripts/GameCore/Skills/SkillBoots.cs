using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillBoots : ISkill
    {
        public ItemType itemType => ItemType.Boots;

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillBoots;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillBoots = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillBoots += value;
        }

    }
}
