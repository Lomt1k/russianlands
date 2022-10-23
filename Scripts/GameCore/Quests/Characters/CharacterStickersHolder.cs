using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public static class CharacterStickersHolder
    {
        private static Dictionary<CharacterType, CharacterStickersSet> _characterStickers;

        public static string? GetSticker(CharacterType characterType, Emotion emotion)
        {
            if (_characterStickers.TryGetValue(characterType, out var charStickers))
            {
                return charStickers.GetStickerFileId(emotion);
            }
            return null;
        }

        public static async Task StickersUpdate()
        {
            Program.logger.Info("Stickers update started...");
            RefillStickersInfo();
            foreach (var characterStickers in _characterStickers.Values)
            {
                await characterStickers.SetupFileIdsFromStickerSet();
            }
            Program.logger.Info("Stickers update completed");
        }

        private static void RefillStickersInfo()
        {
            _characterStickers = new Dictionary<CharacterType, CharacterStickersSet>
            {
                {
                    CharacterType.Vasilisa, new CharacterStickersSet("russlandbot")
                            .RegisterSticker(Emotion.Idle, 0)
                },
                {
                    CharacterType.Dobrynya, new CharacterStickersSet("russlandbot")
                },
                {
                    CharacterType.Magus_Oldman, new CharacterStickersSet("russlandbot")
                },
            };
        }

    }

}
