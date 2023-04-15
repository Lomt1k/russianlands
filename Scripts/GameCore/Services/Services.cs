namespace TextGameRPG.Scripts.GameCore.Services
{
    using Battles;
    using System;
    using System.Collections.Generic;
    using TextGameRPG.Scripts.Bot;
    using TextGameRPG.Scripts.Bot.Sessions;
    using TextGameRPG.Scripts.GameCore.Services.Sending;

    public static class Services
    {
        private static Dictionary<Type, Service> _instances = new Dictionary<Type, Service>();

        static Services()
        {
            Register(new GameData.GameDataHolder());
            Register(new SessionManager());
            Register(new BattleManager());            
            Register(new PerformanceManager());
            Register(new NotificationsManager());
            Register(new MessageSequencer());
            Register(new MessageSender());
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

        public static void OnBotStarted(TelegramBot bot)
        {
            foreach (var singletone in _instances.Values)
            {
                singletone.OnBotStarted(bot);
            }
        }

        public static void OnBotStopped(TelegramBot bot)
        {
            foreach (var singletone in _instances.Values)
            {
                singletone.OnBotStopped(bot);
            }
        }

        

    }
}
