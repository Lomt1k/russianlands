using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using FastTelegramBot.DataTypes;
using MarkOne.Scripts.Bot;

namespace MarkOne.Scripts.GameCore.Services.Sending;

public class MessageSequencer : Service
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _sendMessagesDict = new();
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _editMessagesDict = new();
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _sendStickerDict = new();

    private int _currentSecond;

    public byte SendMessageLimit { get; private set; }
    public byte EditMessageLimit { get; private set; }
    public byte SendStickerLimit { get; private set; }

    public MessageSequencer()
    {
        EverySecondUpdateAsync().FastAwait();
    }

    private async Task EverySecondUpdateAsync()
    {
        var nextUpdateTime = DateTime.UtcNow.AddSeconds(1);
        while (true)
        {
            var delay = (nextUpdateTime - DateTime.UtcNow).Milliseconds;
            nextUpdateTime = nextUpdateTime.AddSeconds(1);
            await Task.Delay(delay).FastAwait();
            var prevSecond = _currentSecond;
            _currentSecond++;

            RemoveDataBySecond(prevSecond);
        }
    }

    public override Task OnBotStarted()
    {
        var sendingLimits = BotController.config.sendingLimits;
        SendMessageLimit = sendingLimits.sendMessagePerSecondLimit;
        EditMessageLimit = sendingLimits.editMessagePerSecondLimit;
        SendStickerLimit = sendingLimits.sendStickerPerSecondLimit;
        return Task.CompletedTask;
    }

    public int GetDelayForSendMessage(string messageText)
    {
        return GetDelayForSomething(_sendMessagesDict, SendMessageLimit, messageText);
    }

    public int GetDelayForEditMessage(string messageText)
    {
        return GetDelayForSomething(_editMessagesDict, EditMessageLimit, messageText);
    }

    public int GetDelayForSendSticker(FileId stickerFileId)
    {
        return GetDelayForSomething(_sendStickerDict, SendStickerLimit, stickerFileId.ToString());
    }


    #region calculation logic

    private int GetDelayForSomething(ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> dictionary, byte limit, string key)
    {
        var resultDelay = 0;
        var i = _currentSecond;
        while (true)
        {
            var dataDict = GetDataBySecond(dictionary, i);
            var count = GetCountByKey(dataDict, key);
            if (count >= limit)
            {
                resultDelay += 1_000;
                i++;
                continue;
            }

            IncreaseCountByKey(dataDict, key);
            break;
        }
        return resultDelay;
    }

    private ConcurrentDictionary<string, byte> GetDataBySecond(ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> dictionary, int second)
    {
        return dictionary.GetOrAdd(second, (sec) =>
        {
            var result = new ConcurrentDictionary<string, byte>();
            dictionary.TryAdd(second, result);
            return result;
        });
    }

    private void RemoveDataBySecond(int second)
    {
        RemoveDataBySecond(_sendMessagesDict, second);
        RemoveDataBySecond(_editMessagesDict, second);
        RemoveDataBySecond(_sendStickerDict, second);
    }

    private void RemoveDataBySecond(ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> dictionary, int second)
    {
        dictionary.TryRemove(second, out _);
    }

    private byte GetCountByKey(ConcurrentDictionary<string, byte> dataDictionary, string key)
    {
        if (dataDictionary.TryGetValue(key, out var count))
        {
            return count;
        }
        return 0;
    }

    private void IncreaseCountByKey(ConcurrentDictionary<string, byte> dataDictionary, string key)
    {
        if (dataDictionary.ContainsKey(key))
        {
            dataDictionary[key]++;
        }
        else
        {
            dataDictionary.TryAdd(key, 1);
        }
    }

    #endregion

}
