using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillStick : ISkill
    {
        public ItemType itemType => ItemType.Stick;

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillStick;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillStick = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillStick += value;
        }

    }
}
