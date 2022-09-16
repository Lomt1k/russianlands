using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.TelegramBot;
using TextGameRPG.Scripts.TelegramBot.DataBase.SerializableData;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Resources
{
    public class PlayerResources
    {
        private static ResourcesDictionary resourceDictionary = new ResourcesDictionary();
        private const int maxResourcesInRow = 3;

        private GameSession _session;
        private ProfileData _profileData;

        public PlayerResources(GameSession session)
        {
            _session = session;
            _profileData = session.profile.data;
        }

        /// <returns>Информация обо всех имеющихся у игрока основных видах ресурсов в готовом для отображения виде</returns>
        public string GetGeneralResourcesView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localizations.Localization.Get(_session, "resource_header_resources"));
            var generalResources = resourceDictionary.GetGeneralResourceTypes();

            int elementsInCurrentRow = 0;
            foreach (var resourceType in generalResources)
            {
                if (!IsUnlocked(resourceType))
                    continue;

                if (elementsInCurrentRow == maxResourcesInRow)
                {
                    sb.AppendLine();
                    elementsInCurrentRow = 0;
                }
                if (elementsInCurrentRow > 0)
                {
                    sb.Append(Emojis.bigSpace);
                }

                sb.Append(GetResourceView(resourceType));
                elementsInCurrentRow++;
            }

            return sb.ToString();
        }

        /// <returns>Количество ресурсов в готовом для отображения виде (с иконкой ресурса + сокращенно)</returns>
        public string GetResourceView(ResourceType resourceType)
        {
            return resourceType.GetShortView(GetValue(resourceType));
        }

        /// <returns>Количество имеющихся у игрока ресурсов указанного типа</returns>
        public int GetValue(ResourceType resourceType)
        {
            return resourceDictionary[resourceType].GetValue(_profileData);
        }

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
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

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <param name="requiredResources">Требуемые ресурсы</param>
        /// <param name="notEnoughResources">Ресурсы, которых не хватило для успешной покупки</param>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
        public bool TryPurchase(Dictionary<ResourceType, int> requiredResources, out Dictionary<ResourceType, int> notEnoughResources)
        {
            notEnoughResources = new Dictionary<ResourceType, int>();
            if (!HasEnough(requiredResources))
            {
                foreach (var resource in requiredResources)
                {
                    var resourceType = resource.Key;
                    var requiredValue = resource.Value;
                    var playerValue = resourceDictionary[resourceType].GetValue(_profileData);
                    if (playerValue < requiredValue)
                    {
                        var notEnoughValue = requiredValue - playerValue;
                        notEnoughResources.Add(resourceType, notEnoughValue);
                    }
                }
                return false;
            }

            return TryPurchase(requiredResources);
        }

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
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

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <param name="resourceType">Тип ресурса</param>
        /// <param name="requiredValue">Требуемое количество ресурса</param>
        /// <param name="notEnoughValue">Количество ресурса, которого не хватило для успешной покупки</param>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
        public bool TryPurchase(ResourceType resourceType, int requiredValue, out int notEnoughValue)
        {
            notEnoughValue = 0;
            if (!HasEnough(resourceType, requiredValue))
            {
                var playerValue = resourceDictionary[resourceType].GetValue(_profileData);
                notEnoughValue = requiredValue - playerValue;
                return false;
            }

            return TryPurchase(resourceType, requiredValue);
        }

        /// <summary>
        /// Есть ли у игрока нужное количество ресурсов, перечисленных в словаре
        /// </summary>
        public bool HasEnough(Dictionary<ResourceType,int> requiredResources)
        {
            foreach (var resource in requiredResources)
            {
                if (!HasEnough(resource.Key, resource.Value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Есть ли у игрока нужное количество ресурсов конкретного типа
        /// </summary>
        public bool HasEnough(ResourceType resourceType, int requiredValue)
        {
            return GetValue(resourceType) >= requiredValue;
        }

        /// <summary>
        /// Доступен ли игроку данный вид ресурса
        /// </summary>
        public bool IsUnlocked(ResourceType resourceType)
        {
            return resourceDictionary[resourceType].IsUnlocked(_profileData);
        }

        /// <summary>
        /// Добавляет ресурс даже если будет превышен лимит хранилища
        /// </summary>
        public void ForceAdd(ResourceType resourceType, int value)
        {
            var resource = resourceDictionary[resourceType];
            var currentValue = resource.GetValue(_profileData);

            var canBeAdded = int.MaxValue - currentValue;
            var reallyAdded = value > canBeAdded ? canBeAdded : value;
            resourceDictionary[resourceType].AddValue(_profileData, reallyAdded);
        }

        /// <summary>
        /// Добавляет ресурсы даже если будет превышен лимит хранилища
        /// </summary>
        public void ForceAdd(Dictionary<ResourceType, int> resources)
        {
            foreach (var resource in resources)
            {
                ForceAdd(resource.Key, resource.Value);
            }
        }

        /// <summary>
        /// Добавляет ресурс, но не превышает лимит хранилища
        /// </summary>
        /// <returns>Сколько по факту было добавлено</returns>
        public int Add(ResourceType resourceType, int value)
        {
            var resource = resourceDictionary[resourceType];
            var currentValue = resource.GetValue(_profileData);
            var maxValue = resource.GetResourceLimit(_profileData);

            var canBeAdded = currentValue > maxValue ? 0 : maxValue - currentValue;
            var reallyAdded = value > canBeAdded ? canBeAdded : value;
            resource.AddValue(_profileData, reallyAdded);

            return reallyAdded;
        }

        /// <summary>
        /// Добавляет ресурсs, но не превышает лимит хранилища
        /// </summary>
        /// <returns>Сколько по факту было добавлено</returns>
        public Dictionary<ResourceType, int> Add(Dictionary<ResourceType, int> resources)
        {
            var reallyAdded = new Dictionary<ResourceType, int>();
            foreach (var resource in resources)
            {
                var value = Add(resource.Key, resource.Value);
                reallyAdded.Add(resource.Key, value);
            }
            return reallyAdded;
        }


    }
}
