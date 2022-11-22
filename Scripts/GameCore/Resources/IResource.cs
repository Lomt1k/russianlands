using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public interface IResource
    {
        public ResourceType resourceType { get; }

        int GetValue(ProfileData profileData);
        void SetValue(ProfileData profileData, int value);
        void AddValue(ProfileData profileData, int value);
        bool IsUnlocked(GameSession session);
        int GetResourceLimit(GameSession session);
    }
}
