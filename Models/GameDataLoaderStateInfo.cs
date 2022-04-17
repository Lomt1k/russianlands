using System;

namespace TextGameRPG.Models
{
    public class GameDataLoaderStateInfo
    {
        public string startTime { get; }
        public string info { get; }

        public string fullInfo => $"{startTime} {info}";

        public GameDataLoaderStateInfo(string _info)
        {
            startTime = $"[{DateTime.Now.ToLongTimeString()}]";
            info = _info;
        }
    }

}
