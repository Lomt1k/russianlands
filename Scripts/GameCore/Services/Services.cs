namespace TextGameRPG.Scripts.GameCore.Services
{
    using Battles;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TextGameRPG.Scripts.Bot;
    using TextGameRPG.Scripts.Bot.Sessions;
    using TextGameRPG.Scripts.GameCore.Services.Mobs;
    using TextGameRPG.Scripts.GameCore.Services.Sending;

    public static class Services
    {
        private static Dictionary<Type, Service> _instances = new Dictionary<Type, Service>();

        static Services()
        {
            Register(new GameData.GameDataHolder());
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
}
