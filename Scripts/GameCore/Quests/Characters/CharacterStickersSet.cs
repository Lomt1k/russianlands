using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TextGameRPG.Scripts.GameCore.Quests.Characters
{
    public class StickerInfo
    {
        public int setPosition { get; private set; }
        public string? fileId { get; private set; }

        public StickerInfo(int _setPosition)
        {
            setPosition = _setPosition;
        }

        public void SetupFileId(string _fileId)
        {
            fileId = _fileId;
        }
    }

    public class CharacterStickersSet
    {
        private readonly Dictionary<Emotion, StickerInfo> _stickersDictionary;
        public string stickerSetName { get; private set; }

        public CharacterStickersSet(string _stickerSetName)
        {
            _stickersDictionary = new Dictionary<Emotion, StickerInfo>();
            stickerSetName = _stickerSetName;
        }

        public CharacterStickersSet RegisterSticker(Emotion emotion, int stickerPosition)
        {
            _stickersDictionary[emotion] = new StickerInfo(stickerPosition);
            return this;
        }

        public string? GetStickerFileId(Emotion emotion)
        {
            if (_stickersDictionary.TryGetValue(emotion, out var stickerInfo))
            {
                return stickerInfo.fileId;
            }
            return null;
        }

        public async Task SetupFileIdsFromStickerSet()
        {
            var botClient = Bot.TelegramBot.instance.client;
            var stickerSet = await botClient.GetStickerSetAsync(stickerSetName).ConfigureAwait(false);
            if (stickerSet == null)
            {
                Program.logger.Error($"Not found stickerset '{stickerSetName}' on telegram servers");
                return;
            }

            var stickersArray = stickerSet.Stickers;
            foreach (var stickerInfo in _stickersDictionary.Values)
            {
                if (stickerInfo.setPosition < stickersArray.Length)
                {
                    var sticker = stickersArray[stickerInfo.setPosition];
                    if (sticker != null)
                    {
                        stickerInfo.SetupFileId(sticker.FileId);
                        continue;
                    }                    
                }
                Program.logger.Error($"Not found sticker at position {stickerInfo.setPosition} in stickerset '{stickerSetName}'");
            }
        }

    }
}
