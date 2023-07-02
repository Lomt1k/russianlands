using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MarkOne.Scripts.GameCore.Resources;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Units.Mobs;

namespace MarkOne.Scripts.GameCore.Services.Mobs;

public class CrossroadsMobsManager : Service
{
    private static readonly ServerDailyDataManager serverDailyDataManager = ServiceLocator.Get<ServerDailyDataManager>();
    private static readonly NotificationsManager notificationsManager = ServiceLocator.Get<NotificationsManager>();
    private static readonly MobFactory mobFactory = ServiceLocator.Get<MobFactory>();

    private Dictionary<MobDifficulty, CrossroadsMobPack> _mobPacks = new();

    public CrossroadsMobPack this[MobDifficulty mobDifficulty] => _mobPacks[mobDifficulty];

    public bool isMobsReady { get; private set; }

    public CrossroadsMobsManager()
    {
        serverDailyDataManager.onStartWithOldDay += OnStartWithOldDay;
        serverDailyDataManager.onStartNewDay += OnStartNewDay;
    }

    private void OnStartWithOldDay()
    {
        isMobsReady = false;
        LoadMobPacks();
    }

    private void OnStartNewDay(DateTime oldDate, DateTime newDate)
    {
        isMobsReady = false;
        CreateNewMobPacks();
    }

    private async void LoadMobPacks()
    {
        var jsonStr = await serverDailyDataManager.GetStringValue("crossroadsMobs", string.Empty).FastAwait();
        if (string.IsNullOrEmpty(jsonStr))
        {
            CreateNewMobPacks();
            return;
        }
        var loadedMobPacks = JsonConvert.DeserializeObject<Dictionary<MobDifficulty, CrossroadsMobPack>>(jsonStr);
        if (loadedMobPacks is null)
        {
            Program.logger.Error("CrossroadsMobsManager: Error when try to desealize 'crossroadsMobs'. Creating new mobs...");
            CreateNewMobPacks();
            return;
        }
        _mobPacks = loadedMobPacks;
        Program.logger.Info("Crossroads mobs loaded");
        isMobsReady = true;
    }

    private async void CreateNewMobPacks()
    {
        _mobPacks.Clear();
        foreach (MobDifficulty difficulty in Enum.GetValues(typeof(MobDifficulty)))
        {
            if (difficulty < MobDifficulty.HALL_5_START)
            {
                continue;
            }
            _mobPacks[difficulty] = GenerateMobPack(difficulty);
        }
        var jsonStr = JsonConvert.SerializeObject(_mobPacks);
        await serverDailyDataManager.SetStringValue("crossroadsMobs", jsonStr).FastAwait();
        Program.logger.Info("New crossroads mobs created");
        isMobsReady = true;
    }

    private CrossroadsMobPack GenerateMobPack(MobDifficulty difficulty)
    {
        var result = new Dictionary<int, CrossroadsMobData[]>();
        for (var setId = 0; setId < CrossroadsMobPack.MOB_SETS_IN_ONE_PACK; setId++)
        {
            var crossId = setId + 1;
            var array = new CrossroadsMobData[3];
            var excludeNames = new List<string>();
            var excludeFruits = new List<ResourceId>();
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = mobFactory.GenerateMobForCrossroads(difficulty, crossId, excludeNames, excludeFruits);
                excludeNames.Add(array[i].localizationKey);
                excludeFruits.Add(array[i].fruitId);
            }
            result[setId] = array;
        }
        return new CrossroadsMobPack(result);
    }

}
