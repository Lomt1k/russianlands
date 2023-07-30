using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services.Battles;
using MarkOne.Scripts.GameCore.Services.DailyDataManager;
using MarkOne.Scripts.GameCore.Services.DailyDataManagers;
using MarkOne.Scripts.GameCore.Services.Mobs;
using MarkOne.Scripts.GameCore.Services.Sending;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Shop.Offers;

namespace MarkOne.Scripts.GameCore.Services;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, Service> _instances = new Dictionary<Type, Service>();

    static ServiceLocator()
    {
        Register(new GameData.GameDataHolder());
        // ServerDailyDataManager должен быть как можно выше
        Register(new ServerDailyDataManager());

        Register(new SessionManager());
        Register(new SessionExceptionHandler());
        Register(new BattleManager());
        Register(new PerformanceManager());
        Register(new Payments.PaymentManager());
        Register(new OffersManager());
        Register(new NotificationsManager());
        Register(new MessageSequencer());
        Register(new MessageSender());
        Register(new DailyRemindersManager());
        Register(new MobFactory());
        Register(new LocationMobsManager());
        Register(new ProfileDailyDataManager());
        Register(new CrossroadsMobsManager());
        Register(new Arena.ArenaMatchMaker());
        Register(new FakePlayers.FakePlayersFactory());
        Register(new News.NewsService());
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
