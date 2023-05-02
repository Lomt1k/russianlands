using System.Threading.Tasks;
using Telegram.Bot;

namespace MarkOne.Scripts.GameCore.Quests.Characters;

public class StickerInfo
{
    public string setName { get; }
    public int setPosition { get; }
    public string? fileId { get; private set; }

    public StickerInfo(string _stickerSetName, int _setPosition)
    {
        setName = _stickerSetName;
        setPosition = _setPosition;
    }

    public async Task SetupFileIdFromStickerSet()
    {
        var botClient = Bot.BotController.botClient;
        var stickerSet = await botClient.GetStickerSetAsync(setName).FastAwait();
        if (stickerSet == null)
        {
            Program.logger.Error($"Not found stickerset '{setName}' on telegram servers");
            return;
        }

        var stickersArray = stickerSet.Stickers;
        if (setPosition < stickersArray.Length)
        {
            var sticker = stickersArray[setPosition];
            if (sticker != null)
            {
                fileId = sticker.FileId;
                return;
            }
        }
        Program.logger.Error($"Not found sticker at position {setPosition} in stickerset '{setName}'");
    }

}
