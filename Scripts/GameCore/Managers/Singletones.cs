namespace TextGameRPG.Scripts.GameCore.Managers
{
    using Battles;
    using System;
    using System.Collections.Generic;

    public static class Singletones
    {
        private static Dictionary<Type, GlobalManager> _instances = new Dictionary<Type, GlobalManager>();

        static Singletones()
        {
            RegisterSingletone<BattleManager>();
            RegisterSingletone<PerformanceManager>();
            RegisterSingletone<NotificationsManager>();
        }

        private static T RegisterSingletone<T>() where T : GlobalManager, new()
        {
            var manager = new T();
            _instances.Add(typeof(T), manager);
            return manager;
        }

        public static T Get<T>() where T : GlobalManager
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
