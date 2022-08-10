using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceArrows : IResource
    {
        public ResourceType resourceType => ResourceType.Arrows;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceArrows;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceArrows = value;
        }
    }
}
