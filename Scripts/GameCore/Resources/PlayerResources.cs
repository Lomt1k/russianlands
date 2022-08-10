using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class PlayerResources
    {
        private static ResourcesDictionary resourceDictionary = new ResourcesDictionary();

        private GameSession _session;
        private ProfileData _profileData;

        public PlayerResources(GameSession session)
        {
            _session = session;
            _profileData = session.profile.data;
        }

        public string GetGeneralResourcesView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localizations.Localization.Get(_session, "unit_view_header_resources"));
            sb.AppendLine(GetResourceView(ResourceType.Gold) 
                + Emojis.bigSpace + GetResourceView(ResourceType.Food)
                + Emojis.bigSpace + GetResourceView(ResourceType.Diamonds));

            return sb.ToString();
        }

        public string GetResourceView(ResourceType resourceType)
        {
            return Emojis.resources[resourceType] + Emojis.space + GetValue(resourceType).View();
        }

        public int GetValue(ResourceType resourceType)
        {
            return resourceDictionary[resourceType].GetValue(_profileData);
        }

        public bool TryPurchase(Dictionary<ResourceType, int> requiredResources)
        {
            if (!HasEnough(requiredResources))
                return false;

            foreach (var resource in requiredResources)
            {
                var resourceType = resource.Key;
                var requiredValue = resource.Value;
                var newValue = GetValue(resourceType) - requiredValue;
                resourceDictionary[resourceType].SetValue(_profileData, newValue);
            }
            return true;
        }

        public bool TryPurchase(ResourceType resourceType, int requiredValue)
        {
            bool success = HasEnough(resourceType, requiredValue);
            if (success)
            {
                var newValue = GetValue(resourceType) - requiredValue;
                resourceDictionary[resourceType].SetValue(_profileData, newValue);
            }
            return success;
        }

        public bool HasEnough(Dictionary<ResourceType,int> requiredResources)
        {
            foreach (var resource in requiredResources)
            {
                if (!HasEnough(resource.Key, resource.Value))
                    return false;
            }
            return true;
        }

        public bool HasEnough(ResourceType resourceType, int requiredValue)
        {
            return GetValue(resourceType) >= requiredValue;
        }


    }
}
