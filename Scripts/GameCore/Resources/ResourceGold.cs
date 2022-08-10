using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceGold : IResource
    {
        public ResourceType resourceType => ResourceType.Gold;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceGold;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceGold = value;
        }
    }
}
