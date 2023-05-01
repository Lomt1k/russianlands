using System;
using System.Collections.Generic;
using System.Text;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.DataBase.SerializableData;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Localizations;

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

        /// <returns>Информация обо всех имеющихся у игрока основных видах ресурсов в готовом для отображения виде</returns>
        public string GetGeneralResourcesView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(_session, "resource_header_ours"));
            var generalResources = resourceDictionary.GetGeneralResourceIds();

            var dictionary = new Dictionary<ResourceId,int>();
            foreach (var resourceId in generalResources)
            {
                if (!IsUnlocked(resourceId))
                    continue;

                dictionary.Add(resourceId, GetValue(resourceId));
            }

            sb.Append(ResourceHelper.GetCompactResourcesView(dictionary));
            return sb.ToString();
        }

        /// <returns>Информация о ресурсах для крафта в готовом для отображения виде</returns>
        public string GetCraftResourcesView()
        {
            var sb = new StringBuilder();
            sb.Append(ResourceId.CraftPiecesCommon.GetEmoji() + Localization.Get(_session, "resource_header_our_materials"));
            var craftResources = resourceDictionary.GetCraftResourceIds();

            foreach (var resourceId in craftResources)
            {
                sb.AppendLine();
                var localization = Localization.Get(_session, "resource_shortname_" + resourceId.ToString().ToLower());
                sb.Append($"{localization} " + GetValue(resourceId).ToString().Bold());
            }

            return sb.ToString();
        }

        /// <returns>Информация о ресурсах для прокачки навыков для отображения виде</returns>
        public string GetFruitsView()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Localization.Get(_session, "resource_header_our_fruits"));
            var fruitTypes = resourceDictionary.GetFruitTypes();

            int i = 0;
            foreach (var resourceId in fruitTypes)
            {
                if (i > 0)
                {
                    sb.Append(i % 4 == 0 ? Environment.NewLine : Emojis.middleSpace);
                }
                sb.Append(resourceId.GetEmoji() + GetValue(resourceId).ToString());
                i++;
            }

            return sb.ToString();
        }

        /// <returns>Количество имеющихся у игрока ресурсов указанного типа</returns>
        public int GetValue(ResourceId resourceId)
        {
            return resourceDictionary[resourceId].GetValue(_profileData);
        }

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
        public bool TryPurchase(Dictionary<ResourceId, int> requiredResources)
        {
            if (!HasEnough(requiredResources))
                return false;

            foreach (var resource in requiredResources)
            {
                var resourceId = resource.Key;
                var requiredValue = resource.Value;
                var newValue = GetValue(resourceId) - requiredValue;
                resourceDictionary[resourceId].SetValue(_profileData, newValue);
            }
            return true;
        }

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <param name="requiredResources">Требуемые ресурсы</param>
        /// <param name="notEnoughResources">Ресурсы, которых не хватило для успешной покупки</param>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
        public bool TryPurchase(Dictionary<ResourceId, int> requiredResources, out Dictionary<ResourceId, int> notEnoughResources)
        {
            notEnoughResources = new Dictionary<ResourceId, int>();
            if (!HasEnough(requiredResources))
            {
                foreach (var resource in requiredResources)
                {
                    var resourceId = resource.Key;
                    var requiredValue = resource.Value;
                    var playerValue = resourceDictionary[resourceId].GetValue(_profileData);
                    if (playerValue < requiredValue)
                    {
                        var notEnoughValue = requiredValue - playerValue;
                        notEnoughResources.Add(resourceId, notEnoughValue);
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
        public bool TryPurchase(ResourceId resourceId, int requiredValue)
        {
            bool success = HasEnough(resourceId, requiredValue);
            if (success)
            {
                var newValue = GetValue(resourceId) - requiredValue;
                resourceDictionary[resourceId].SetValue(_profileData, newValue);
            }
            return success;
        }

        /// <summary>
        /// Попытаться произвести покупку со списанием указанных ресурсов
        /// </summary>
        /// <param name="resourceId">Тип ресурса</param>
        /// <param name="requiredValue">Требуемое количество ресурса</param>
        /// <param name="notEnoughValue">Количество ресурса, которого не хватило для успешной покупки</param>
        /// <returns>Вернет true, если списание средств прошло успешно</returns>
        public bool TryPurchase(ResourceId resourceId, int requiredValue, out int notEnoughValue)
        {
            notEnoughValue = 0;
            if (!HasEnough(resourceId, requiredValue))
            {
                var playerValue = resourceDictionary[resourceId].GetValue(_profileData);
                notEnoughValue = requiredValue - playerValue;
                return false;
            }

            return TryPurchase(resourceId, requiredValue);
        }

        /// <summary>
        /// Есть ли у игрока нужное количество ресурсов, перечисленных в словаре
        /// </summary>
        public bool HasEnough(Dictionary<ResourceId,int> requiredResources)
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
        public bool HasEnough(ResourceId resourceId, int requiredValue)
        {
            return GetValue(resourceId) >= requiredValue;
        }

        /// <summary>
        /// Доступен ли игроку данный вид ресурса
        /// </summary>
        public bool IsUnlocked(ResourceId resourceId)
        {
            return resourceDictionary[resourceId].IsUnlocked(_session);
        }

        /// <summary>
        /// Добавляет ресурс даже если будет превышен лимит хранилища
        /// </summary>
        public void ForceAdd(ResourceId resourceId, int value)
        {
            var resource = resourceDictionary[resourceId];
            var currentValue = resource.GetValue(_profileData);

            var canBeAdded = int.MaxValue - currentValue;
            var reallyAdded = value > canBeAdded ? canBeAdded : value;
            resource.AddValue(_profileData, reallyAdded);
        }

        /// <summary>
        /// Добавляет ресурсы даже если будет превышен лимит хранилища
        /// </summary>
        public void ForceAdd(Dictionary<ResourceId, int> resources)
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
        public int Add(ResourceId resourceId, int value)
        {
            var resource = resourceDictionary[resourceId];
            var currentValue = resource.GetValue(_profileData);
            var maxValue = resource.GetResourceLimit(_session);

            var canBeAdded = currentValue > maxValue ? 0 : maxValue - currentValue;
            var reallyAdded = value > canBeAdded ? canBeAdded : value;
            resource.AddValue(_profileData, reallyAdded);

            return reallyAdded;
        }

        /// <summary>
        /// Добавляет ресурсы, но не превышает лимит хранилища
        /// </summary>
        /// <returns>Сколько по факту было добавлено</returns>
        public Dictionary<ResourceId, int> Add(Dictionary<ResourceId, int> resources)
        {
            var reallyAdded = new Dictionary<ResourceId, int>();
            foreach (var resource in resources)
            {
                var value = Add(resource.Key, resource.Value);
                reallyAdded.Add(resource.Key, value);
            }
            return reallyAdded;
        }

        /// <returns>Максимальное количество ресурсов (лимит хранилища)</returns>
        public int GetResourceLimit(ResourceId resourceId)
        {
            return resourceDictionary[resourceId].GetResourceLimit(_session);
        }

        /// <returns>Достигнут ли лимит ресурсов в хранилище</returns>
        public bool IsResourceLimitReached(ResourceId resourceId)
        {
            return GetValue(resourceId) >= GetResourceLimit(resourceId);
        }


    }
}
