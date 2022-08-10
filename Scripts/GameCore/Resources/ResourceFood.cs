using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class ResourceFood : IResource
    {
        public ResourceType resourceType => ResourceType.Food;

        public int GetValue(ProfileData profileData)
        {
            return profileData.resourceFood;
        }

        public void SetValue(ProfileData profileData, int value)
        {
            profileData.resourceFood = value;
        }
    }
}
