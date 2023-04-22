using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Localizations
{
    [SQLite.StoreAsText]
    public enum LanguageCode
    {
        EN = 0,
        RU = 1
    }

    public static class LanguageCodeExtensions
    {
        public static string GetLanguageView(this LanguageCode code)
        {
            return code switch
            {
                LanguageCode.EN => Emojis.FlagBritain + "English",
                LanguageCode.RU => Emojis.FlagRussia + "Русский",
                _ => "unknown_language"
            };
        }
    }
}
