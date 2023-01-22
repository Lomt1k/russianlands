using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    public interface ISkill
    {
        public ItemType itemType { get; }

        byte GetValue(ProfileData profileData);
        void SetValue(ProfileData profileData, byte value);
        void AddValue(ProfileData profileData, byte value);
    }
}
