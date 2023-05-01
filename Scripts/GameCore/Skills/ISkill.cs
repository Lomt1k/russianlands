using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Resources;

namespace TextGameRPG.Scripts.GameCore.Skills
{
    public interface ISkill
    {
        public ItemType itemType { get; }
        public ResourceId[] requiredFruits { get; }

        byte GetValue(ProfileData profileData);
        void SetValue(ProfileData profileData, byte value);
        void AddValue(ProfileData profileData, byte value);
    }
}
