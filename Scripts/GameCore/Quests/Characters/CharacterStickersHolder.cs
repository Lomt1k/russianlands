using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public static class CharacterStickersHolder
    {
        private static Dictionary<CharacterType, StickerInfo> _characterStickers;

        public static string? GetSticker(CharacterType characterType)
        {
            if (_characterStickers.TryGetValue(characterType, out var stickerInfo))
            {
                return stickerInfo.fileId;
            }
            return null;
        }

        public static async Task StickersUpdate()
        {
            Program.logger.Info("Stickers update started...");
            RefillStickersInfo();
            foreach (var characterStickers in _characterStickers.Values)
            {
                await characterStickers.SetupFileIdFromStickerSet().FastAwait();
            }
            Program.logger.Info("Stickers update completed");
        }

        private static void RefillStickersInfo()
        {
            _characterStickers = new Dictionary<CharacterType, StickerInfo>
            {
                { CharacterType.Vasilisa, new StickerInfo("russlandbot", 0) },
                { CharacterType.Dobrynya, new StickerInfo("russlandbot", 1) },
                { CharacterType.Magus_Oldman, new StickerInfo("russlandbot", 2) },
                { CharacterType.Demon_Cracy, new StickerInfo("russlandbot", 3) },
                { CharacterType.Nightingale, new StickerInfo("russlandbot", 4) },
                { CharacterType.Yaga, new StickerInfo("russlandbot", 5) },
                { CharacterType.Rogue_Mage, new StickerInfo("russlandbot", 6) },
                { CharacterType.Wood_Goblin, new StickerInfo("russlandbot", 7) },
                { CharacterType.Koschey, new StickerInfo("russlandbot", 8) },
                { CharacterType.Kikimora, new StickerInfo("russlandbot", 9) },
                { CharacterType.Aquatic, new StickerInfo("russlandbot", 10) },
                { CharacterType.Dovahkiin, new StickerInfo("russlandbot", 11) },
                { CharacterType.Gorinich, new StickerInfo("russlandbot", 12) },
                { CharacterType.GorinichTwoHeads, new StickerInfo("russlandbot", 13) },
                { CharacterType.GorinichOneHead, new StickerInfo("russlandbot", 14) },
                { CharacterType.Buffoon, new StickerInfo("russlandbot", 15) },
                { CharacterType.NifNif, new StickerInfo("russlandbot", 16) },
                { CharacterType.NufNuf, new StickerInfo("russlandbot", 17) },
                { CharacterType.NafNaf, new StickerInfo("russlandbot", 18) },                
                { CharacterType.ChocolateKing, new StickerInfo("russlandbot", 19) },
                { CharacterType.OrcLeader, new StickerInfo("russlandbot", 20) },
                { CharacterType.OrcGuardian, new StickerInfo("russlandbot", 21) },
                { CharacterType.Troll, new StickerInfo("russlandbot", 22) },
                { CharacterType.GnomeCommander, new StickerInfo("russlandbot", 23) },
                { CharacterType.OrcTraitor, new StickerInfo("russlandbot", 24) },
                { CharacterType.ElvenKing, new StickerInfo("russlandbot", 25) },
                { CharacterType.Commissioner, new StickerInfo("russlandbot", 26) },
                { CharacterType.RobinBad, new StickerInfo("russlandbot", 27) },
                { CharacterType.Ent, new StickerInfo("russlandbot", 28) },
                { CharacterType.Anariel, new StickerInfo("russlandbot", 29) },
                { CharacterType.Tavrilia, new StickerInfo("russlandbot", 30) },
                { CharacterType.Lovelas, new StickerInfo("russlandbot", 31) },
                { CharacterType.Banker, new StickerInfo("russlandbot", 32) },
                { CharacterType.Eagle, new StickerInfo("russlandbot", 33) },
                { CharacterType.DemonOfWrath, new StickerInfo("russlandbot", 34) },
                { CharacterType.DemonOfGreed, new StickerInfo("russlandbot", 35) },
                { CharacterType.DemonOfLie, new StickerInfo("russlandbot", 36) },
            };
        }

    }

}
