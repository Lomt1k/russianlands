namespace TextGameRPG.Scripts.GameCore.Managers
{
    using Battles;
    using System.Collections.Generic;

    public static class GlobalManagers
    {
        public static BattleManager? battleManager { get; private set; }
        public static PerformanceManager? performanceManager { get; private set; }

        private static List<GlobalManager> _allManagers = new List<GlobalManager>();

        public static void CreateManagers()
        {
            battleManager = CreateManager<BattleManager>();
            performanceManager = CreateManager<PerformanceManager>();
        }

        public static void DisposeManagers()
        {
            foreach (var manager in _allManagers)
            {
                manager.OnDestroy();
            }
            _allManagers.Clear();

            //сбрасываем ссылки
            battleManager = null;
            performanceManager = null;
        }

        private static T CreateManager<T>() where T : GlobalManager, new()
        {
            var manager = new T();
            _allManagers.Add(manager);
            return manager;
        }

    }
}
