using System.Collections.Generic;
using System.Threading.Tasks;

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
                    .RegisterSticker(Emotion.Idle, 2)
                    .RegisterSticker(Emotion.Joyfull, 2)
                    .RegisterSticker(Emotion.Angry, 2)
                    .RegisterSticker(Emotion.Surprised, 2)
                },
                {
                    CharacterType.Dobrynya, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 0)
                    .RegisterSticker(Emotion.Joyfull, 0)
                    .RegisterSticker(Emotion.Angry, 0)
                    .RegisterSticker(Emotion.Surprised, 0)
                },
                {
                    CharacterType.Magus_Oldman, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 1)
                    .RegisterSticker(Emotion.Joyfull, 1)
                    .RegisterSticker(Emotion.Angry, 1)
                    .RegisterSticker(Emotion.Surprised, 1)
                },
                {
                    CharacterType.Demon_Cracy, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 3)
                },
                {
                    CharacterType.Nightingale, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 4)
                },
                {
                    CharacterType.Yaga, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 5)
                    .RegisterSticker(Emotion.Surprised, 5)
                },
                {
                    CharacterType.Wood_Goblin, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 6)
                },
                {
                    CharacterType.Rogue_Mage, new CharacterStickersSet("russlandbot")
                    .RegisterSticker(Emotion.Idle, 7)
                },
            };
        }

    }

}
