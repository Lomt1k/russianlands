using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillScroll : ISkill
    {
        public ItemType itemType => ItemType.Scroll;

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
