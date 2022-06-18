
namespace TextGameRPG.Scripts.TelegramBot.Dialogs.Characters
{
    internal class Character
    {
        public string nameLocalizationKey = string.Empty;

        public Character(string _localizationKey)
        {
            nameLocalizationKey = _localizationKey;
        }

    }
}
