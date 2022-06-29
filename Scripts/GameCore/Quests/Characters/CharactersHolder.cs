using System.Collections.Generic;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public enum CharacterType : int
    {
        None = 0,
        Vasilisa = 100,
        Robber = 200,
    }

    public static class CharacterTypeExtensions
    {
        public static string GetName(this CharacterType characterType, GameSession session)
        {
            return Localizations.Localization.Get(session, $"character_{characterType.ToString().ToLower()}_name");
        }

        public static string GetNameBold(this CharacterType characterType, GameSession session)
        {
            return "<b>" + characterType.GetName(session) + "</b>";
        }
    }

    public static class CharactersHolder
    {
        private static Dictionary<CharacterType, Character> _characters = new Dictionary<CharacterType, Character>
        {
            { CharacterType.Vasilisa, new Character() },
            { CharacterType.Robber, new Character() },
        };


    }
}
