namespace TextGameRPG.Scripts.GameCore.Managers
{
    using Battles;
    using System;
    using System.Collections.Generic;
    using TextGameRPG.Scripts.GameCore.Managers.Sending;

    public static class Singletones
    {
        private static Dictionary<Type, Singletone> _instances = new Dictionary<Type, Singletone>();

        static Singletones()
        {
            RegisterSingletone<GameDataBase.GameDataBase>();
            RegisterSingletone<BattleManager>();
            RegisterSingletone<PerformanceManager>();
            RegisterSingletone<NotificationsManager>();
            RegisterSingletone<MessageSequencer>();
        }

        private static T RegisterSingletone<T>() where T : Singletone, new()
        {
            var manager = new T();
            _instances.Add(typeof(T), manager);
            return manager;
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
