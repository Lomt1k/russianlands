using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.Bot;

namespace TextGameRPG.Scripts.GameCore.Services.Sending
{
    public class MessageSequencer : Service
    {
        private Dictionary<int, Dictionary<string, byte>> _sendMessagesDict = new Dictionary<int, Dictionary<string, byte>>();
        private Dictionary<int, Dictionary<string, byte>> _editMessagesDict = new Dictionary<int, Dictionary<string, byte>>();
        private Dictionary<int, Dictionary<string, byte>> _sendStickerDict = new Dictionary<int, Dictionary<string, byte>>();

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

        public int GetDelayForSendSticker(string stickerFileId)
        {
            return GetDelayForSomething(_sendStickerDict, SendStickerLimit, stickerFileId);
        }


        #region calculation logic

        private int GetDelayForSomething(Dictionary<int, Dictionary<string, byte>> dictionary, byte limit, string key)
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

        private Dictionary<string, byte> GetDataBySecond(Dictionary<int, Dictionary<string, byte>> dictionary, int second)
        {
            if (dictionary.TryGetValue(second, out var result))
            {
                return result;
            }
            result = new Dictionary<string, byte>();
            dictionary[second] = result;
            return result;
        }

        private void RemoveDataBySecond(int second)
        {
            RemoveDataBySecond(_sendMessagesDict, second);
            RemoveDataBySecond(_editMessagesDict, second);
            RemoveDataBySecond(_sendStickerDict, second);
        }

        private void RemoveDataBySecond(Dictionary<int, Dictionary<string, byte>> dictionary, int second)
        {
            dictionary.Remove(second);
        }

        private byte GetCountByKey(Dictionary<string, byte> dataDictionary, string key)
        {
            if (dataDictionary.TryGetValue(key, out var count))
            {
                return count;
            }
            return 0;
        }

        private void IncreaseCountByKey(Dictionary<string, byte> dataDictionary, string key)
        {
            if (dataDictionary.ContainsKey(key))
            {
                dataDictionary[key]++;
            }
            else
            {
                dataDictionary[key] = 1;
            }
        }

        #endregion

    }
}
