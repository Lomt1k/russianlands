using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters;

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

    // LOC_04
    Buffoon = 160,
    NifNif = 170,
    NufNuf = 180,
    NafNaf = 190,
    ChocolateKing = 200,

    // LOC_05
    OrcLeader = 210,
    OrcGuardian = 220,
    Troll = 230,
    GnomeCommander = 240,
    OrcTraitor = 250,
    ElvenKing = 260,

    // LOC_06
    Commissioner = 270,
    RobinBad = 280,
    Ent = 290,
    Anariel = 300,
    Tavrilia = 310,
    Lovelas = 320,

    // LOC_07
    Banker = 330,
    Eagle = 340,
    DemonOfWrath = 350,
    DemonOfGreed = 360,
    DemonOfLie = 370,
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
