using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public interface IResource
    {
        public ResourceType resourceType { get; }

        int GetValue(ProfileData profileData);
        void SetValue(ProfileData profileData, int value);
    }
}
