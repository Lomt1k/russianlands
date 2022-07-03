
namespace TextGameRPG.Scripts.TelegramBot.Managers
{
    using Battles;

    public static class GlobalManagers
    {
        public static BattleManager? battleManager { get; private set; }

        public static void CreateManagers()
        {
            battleManager = CreateManager<BattleManager>();
        }

        public static void DisposeManagers()
        {
            battleManager = null;
        }

        private static T CreateManager<T>() where T : GlobalManager, new()
        {
            var manager = new T();
            manager.OnManagerStart();
            return manager;
        }

    }
}
