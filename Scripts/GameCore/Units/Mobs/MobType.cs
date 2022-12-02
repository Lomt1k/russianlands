using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Units.Mobs
{
    public enum MobType
    {
        Default = 0,
        Strong = 1,
        Boss = 2,
    }

    public static class MobTypeExtensions
    {
        public static string GetView(this MobType mobType, GameSession session)
        {
            return Localizations.Localization.Get(session, "mob_type_" + mobType.ToString().ToLower());
        }
    }
}
