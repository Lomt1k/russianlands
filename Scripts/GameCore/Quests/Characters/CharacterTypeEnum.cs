using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public enum CharacterType : int
    {
        None = 0,
        Vasilisa = 10,
        Dobrynya = 20,
        Magus_Oldman = 30,
        Demon_Cracy = 40,

        // LOC 01
        Nightingale = 50,
        Yaga = 60,
        Rogue_Mage = 70,
        Wood_Goblin = 80,

        // LOC 02
        Koschey = 90,
        Kikimora = 100,
        Aquatic = 110,

        // LOC 03
        Dovahkiin = 120,
        Gorinich = 130,
        GorinichTwoHeads = 140,
        GorinichOneHead = 150,
    }

    public static class CharacterTypeExtensions
    {
        public static string GetName(this CharacterType characterType, GameSession session)
        {
            return Localizations.Localization.Get(session, $"character_{characterType.ToString().ToLower()}_name");
        }

        public static string? GetSticker(this CharacterType characterType)
        {
            return CharacterStickersHolder.GetSticker(characterType);
        }

    }
}
