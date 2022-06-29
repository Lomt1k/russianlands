using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    public enum MobType
    {
        None = 0,
        Humanoid = 1,
        Animal = 2,
        Monster = 3,
        Demon = 4,
        Undead = 5,
    }

    public static class MobTypeExtensions
    {
        public static string GetView(this MobType mobType, GameSession session)
        {
            return Localizations.Localization.Get(session, "mob_type_" + mobType.ToString().ToLower());
        }
    }
}
