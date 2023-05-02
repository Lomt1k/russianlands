﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.Bot.Sessions;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.DailyDataManager;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Services.Mobs;
using MarkOne.Scripts.GameCore.Services.Sending;

namespace MarkOne.Scripts.GameCore.Services;

public static class Services
{
    private static readonly Dictionary<Type, Service> _instances = new Dictionary<Type, Service>();

    static Services()
    {
        Register(new GameData.GameDataHolder());
        // ServerDailyDataManager должен быть как можно выше
        Register(new ServerDailyDataManager());

        Register(new SessionManager());
        Register(new BattleManager());
        Register(new PerformanceManager());
        Register(new NotificationsManager());
        Register(new MessageSequencer());
        Register(new MessageSender());
        Register(new DailyRemindersManager());
        Register(new MobFactory());
        Register(new LocationMobsManager());
        Register(new ProfileDailyDataManager());
        Register(new CrossroadsMobsManager());
    }

    private static void Register<T>(T instance) where T : Service, new()
    {
        _instances.Add(typeof(T), instance);
    }

    public static T Get<T>() where T : Service
    {
        var singletone = _instances[typeof(T)];
        return (T)singletone;
    }

    public static async Task OnBotStarted()
    {
        foreach (var singletone in _instances.Values)
        {
            await singletone.OnBotStarted().FastAwait();
        }
    }

    public static async Task OnBotStopped()
    {
        foreach (var singletone in _instances.Values)
        {
            await singletone.OnBotStopped().FastAwait();
        }
    }



}
