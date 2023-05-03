using System;
using System.Collections.Generic;
using System.Text;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Localizations;
using MarkOne.Scripts.GameCore.Services.BotData.SerializableData;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne.Scripts.GameCore.Resources;

public class PlayerResources
{
    private readonly GameSession _session;
    private readonly ProfileData _profileData;

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
        var generalResources = ResourcesDictionary.GetGeneralResourceIds();

        var resourceDatas = new List<ResourceData>();
        foreach (var resourceId in generalResources)
        {
            if (!IsUnlocked(resourceId))
                continue;

            resourceDatas.Add(new ResourceData(resourceId, GetValue(resourceId)));
        }

        sb.Append(resourceDatas.GetCompactView());
        return sb.ToString();
    }

    /// <returns>Информация о ресурсах для крафта в готовом для отображения виде</returns>
    public string GetCraftResourcesView()
    {
        var sb = new StringBuilder();
        sb.Append(ResourceId.CraftPiecesCommon.GetEmoji() + Localization.Get(_session, "resource_header_our_materials"));
        var craftResources = ResourcesDictionary.GetCraftResourceIds();

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
        var fruitTypes = ResourcesDictionary.GetFruitTypes();

        var i = 0;
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
        return ResourcesDictionary.Get(resourceId).GetValue(_profileData);
    }

    /// <returns>Количество имеющихся у игрока ресурсов в виде ResourceData</returns>
    public ResourceData GetResourceData(ResourceId resourceId)
    {
        return new ResourceData(resourceId, GetValue(resourceId));
    }

    /// <summary>
    /// Попытаться произвести покупку со списанием указанных ресурсов
    /// </summary>
    /// <returns>Вернет true, если списание средств прошло успешно</returns>
    public bool TryPurchase(IEnumerable<ResourceData> requiredResources)
    {
        if (!HasEnough(requiredResources))
            return false;

        foreach (var (resourceId, amount) in requiredResources)
        {
            var newValue = GetValue(resourceId) - amount;
            ResourcesDictionary.Get(resourceId).SetValue(_profileData, newValue);
        }
        return true;
    }

    /// <summary>
    /// Попытаться произвести покупку со списанием указанных ресурсов
    /// </summary>
    /// <param name="requiredResources">Требуемые ресурсы</param>
    /// <param name="notEnoughResources">Ресурсы, которых не хватило для успешной покупки</param>
    /// <returns>Вернет true, если списание средств прошло успешно</returns>
    public bool TryPurchase(IEnumerable<ResourceData> requiredResources, out List<ResourceData> notEnoughResources)
    {
        notEnoughResources = new List<ResourceData>();
        if (!HasEnough(requiredResources))
        {
            foreach (var (resourceId, amount) in requiredResources)
            {
                var playerValue = ResourcesDictionary.Get(resourceId).GetValue(_profileData);
                if (playerValue < amount)
                {
                    var notEnoughValue = amount - playerValue;
                    notEnoughResources.Add(new ResourceData(resourceId, notEnoughValue));
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
    public bool TryPurchase(ResourceData requiredResource)
    {
        var success = HasEnough(requiredResource);
        if (success)
        {
            var newValue = GetValue(requiredResource.resourceId) - requiredResource.amount;
            ResourcesDictionary.Get(requiredResource.resourceId).SetValue(_profileData, newValue);
        }
        return success;
    }

    /// <summary>
    /// Попытаться произвести покупку со списанием указанных ресурсов
    /// </summary>
    /// <param name="requiredResource">Требуемые ресурсы</param>
    /// <param name="notEnoughResource">Количество ресурса, которого не хватило для успешной покупки</param>
    /// <returns>Вернет true, если списание средств прошло успешно</returns>
    public bool TryPurchase(ResourceData requiredResource, out ResourceData notEnoughResource)
    {
        if (!HasEnough(requiredResource))
        {
            var playerValue = ResourcesDictionary.Get(requiredResource.resourceId).GetValue(_profileData);
            notEnoughResource = requiredResource with { amount = requiredResource.amount - playerValue };
            return false;
        }

        notEnoughResource = new();
        return TryPurchase(requiredResource);
    }

    /// <summary>
    /// Есть ли у игрока нужное количество ресурсов, перечисленных в словаре
    /// </summary>
    public bool HasEnough(IEnumerable<ResourceData> requiredResources)
    {
        foreach (var requiredResource in requiredResources)
        {
            if (!HasEnough(requiredResources))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Есть ли у игрока нужное количество ресурсов конкретного типа
    /// </summary>
    public bool HasEnough(ResourceData requiredResource)
    {
        return GetValue(requiredResource.resourceId) >= requiredResource.amount;
    }

    /// <summary>
    /// Доступен ли игроку данный вид ресурса
    /// </summary>
    public bool IsUnlocked(ResourceId resourceId)
    {
        return ResourcesDictionary.Get(resourceId).IsUnlocked(_session);
    }

    /// <summary>
    /// Добавляет ресурс даже если будет превышен лимит хранилища
    /// </summary>
    public void ForceAdd(ResourceData resourceData)
    {
        var resource = ResourcesDictionary.Get(resourceData.resourceId);
        var currentValue = resource.GetValue(_profileData);

        var canBeAdded = int.MaxValue - currentValue;
        var reallyAdded = resourceData.amount > canBeAdded ? canBeAdded : resourceData.amount;
        resource.AddValue(_profileData, reallyAdded);
    }

    /// <summary>
    /// Добавляет ресурсы даже если будет превышен лимит хранилища
    /// </summary>
    public void ForceAdd(IEnumerable<ResourceData> resources)
    {
        foreach (var resource in resources)
        {
            ForceAdd(resource);
        }
    }

    /// <summary>
    /// Добавляет ресурс, но не превышает лимит хранилища
    /// </summary>
    /// <returns>Сколько по факту было добавлено</returns>
    public ResourceData Add(ResourceData resourceData)
    {
        var resource = ResourcesDictionary.Get(resourceData.resourceId);
        var currentValue = resource.GetValue(_profileData);
        var maxValue = resource.GetResourceLimit(_session);

        var canBeAdded = currentValue > maxValue ? 0 : maxValue - currentValue;
        var reallyAdded = resourceData.amount > canBeAdded ? canBeAdded : resourceData.amount;
        resource.AddValue(_profileData, reallyAdded);

        return resourceData with { amount = reallyAdded };
    }

    /// <summary>
    /// Добавляет ресурсы, но не превышает лимит хранилища
    /// </summary>
    /// <returns>Сколько по факту было добавлено</returns>
    public List<ResourceData> Add(IEnumerable<ResourceData> resourceDatas)
    {
        var reallyAdded = new List<ResourceData>();
        foreach (var resourceData in resourceDatas)
        {
            var addedResource = Add(resourceData);
            reallyAdded.Add(addedResource);
        }
        return reallyAdded;
    }

    /// <returns>Максимальное количество ресурсов (лимит хранилища)</returns>
    public int GetResourceLimit(ResourceId resourceId)
    {
        return ResourcesDictionary.Get(resourceId).GetResourceLimit(_session);
    }

    /// <returns>Достигнут ли лимит ресурсов в хранилище</returns>
    public bool IsResourceLimitReached(ResourceId resourceId)
    {
        return GetValue(resourceId) >= GetResourceLimit(resourceId);
    }


}
