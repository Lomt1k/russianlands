using JsonKnownTypes;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.TelegramBot.CallbackData
{
    /// <summary>
    /// ATTENTION: Max Callback Data Length (in JSON): 128 symbols!
    /// </summary>

    [JsonConverter(typeof(JsonKnownTypesConverter<CallbackDataBase>))]
    [JsonDiscriminator(Name = "t")]
    [JsonKnownType(typeof(DialogPanelButtonCallbackData), "dialog")]
    public abstract class CallbackDataBase
    {
    }
}
