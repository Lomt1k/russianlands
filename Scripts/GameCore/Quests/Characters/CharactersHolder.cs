using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public enum CharacterType : int
    {
        None = 0,
        Vasilisa = 100,
    }

    internal static class CharactersHolder
    {
        private static Dictionary<CharacterType, Character> _characters = new Dictionary<CharacterType, Character>
        {
            { CharacterType.Vasilisa, new Character("character_name_vasilisa") }
        };


    }
}
