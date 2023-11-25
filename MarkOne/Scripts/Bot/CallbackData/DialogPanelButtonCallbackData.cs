using Newtonsoft.Json;

namespace MarkOne.Scripts.Bot.CallbackData;

public class DialogPanelButtonCallbackData : CallbackDataBase
{
    [JsonProperty("h")]
    public int aliveQueryHash { get; set; }
    [JsonProperty("b")]
    public int buttonId { get; set; }
}
