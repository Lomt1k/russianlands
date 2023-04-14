namespace TextGameRPG.Scripts.GameCore.Managers
{
    using Battles;
    using System;
    using System.Collections.Generic;
    using TextGameRPG.Scripts.Bot.Sessions;
    using TextGameRPG.Scripts.GameCore.Managers.Sending;

    public static class Singletones
    {
        private static Dictionary<Type, Singletone> _instances = new Dictionary<Type, Singletone>();

        static Singletones()
        {
            RegisterSingletone(new GameDataBase.GameDataBase());
            RegisterSingletone(new SessionManager());
            RegisterSingletone(new BattleManager());
            RegisterSingletone(new PerformanceManager());
            RegisterSingletone(new NotificationsManager());
            RegisterSingletone(new MessageSequencer());
        }

        private static void RegisterSingletone<T>(T instance) where T : Singletone, new()
        {
            _instances.Add(typeof(T), instance);
        }

        public static T Get<T>() where T : Singletone
        {
            var singletone = _instances[typeof(T)];
            return (T)singletone;
        }

        public static void OnBotStarted()
        {
            foreach (var singletone in _instances.Values)
            {
                singletone.OnBotStarted();
            }
        }

        public static void OnBotStopped()
        {
            foreach (var singletone in _instances.Values)
            {
                singletone.OnBotStopped();
            }
        }

        

    }
}
