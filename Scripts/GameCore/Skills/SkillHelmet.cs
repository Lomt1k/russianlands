using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    internal class SkillHelmet : ISkill
    {
        public ItemType itemType => ItemType.Helmet;

        public byte GetValue(ProfileData profileData)
        {
            return profileData.skillHelmet;
        }

        public void SetValue(ProfileData profileData, byte value)
        {
            profileData.skillHelmet = value;
        }

        public void AddValue(ProfileData profileData, byte value)
        {
            profileData.skillHelmet += value;
        }

    }
}
