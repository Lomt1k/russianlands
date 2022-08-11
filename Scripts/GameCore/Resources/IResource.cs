using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public interface IResource
    {
        public ResourceType resourceType { get; }

        int GetValue(ProfileData profileData);
        void SetValue(ProfileData profileData, int value);
        void AddValue(ProfileData profileData, int value);
        bool IsUnlocked(ProfileData profileData);
        int GetResourceLimit(ProfileData profileData);
    }
}
