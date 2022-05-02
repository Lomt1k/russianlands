using System.Collections.Generic;

namespace TextGameRPG.Scripts.TelegramBot
{
    public enum FlagCode
    {
        English,
        Russian
    }

    public static class Emojis
    {
        public static readonly Dictionary<FlagCode, string> flags = new Dictionary<FlagCode, string>()
        {
            { FlagCode.English, "\ud83c\uddec\ud83c\udde7" },
            { FlagCode.Russian, "\ud83c\uddf7\ud83c\uddfa" }
        };

    }
}
