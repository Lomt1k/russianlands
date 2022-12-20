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
                await characterStickers.SetupFileIdFromStickerSet().ConfigureAwait(false);
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
            };
        }

    }

}
