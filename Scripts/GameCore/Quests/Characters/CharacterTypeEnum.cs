using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public enum CharacterType : int
    {
        None = 0,
        Vasilisa = 10,
        Dobrynya = 20,
        Magus_Oldman = 30,
        Demon_Cracy = 40,

        //LOC 01
        Nightingale = 50,
        Yaga = 60,
        Rogue_Mage = 70,
        Wood_Goblin = 80,
    }

    public enum Emotion : byte
    {
        None = 0,
        Idle = 1,
        Joyfull = 2,
        Angry = 3,
        Surprised = 4,
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

        public static string? GetSticker(this CharacterType characterType, Emotion emotion)
        {
            return CharacterStickersHolder.GetSticker(characterType, emotion);
        }

    }
}
