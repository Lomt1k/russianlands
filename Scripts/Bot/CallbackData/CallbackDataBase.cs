using JsonKnownTypes;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.Bot.CallbackData;

/// <summary>
/// ATTENTION: Max Callback Data Length (in JSON): 128 symbols!
/// </summary>

[JsonConverter(typeof(JsonKnownTypesConverter<CallbackDataBase>))]
[JsonDiscriminator(Name = "t")]
[JsonKnownType(typeof(DialogPanelButtonCallbackData), "dialog")]
[JsonKnownType(typeof(BattleTooltipCallbackData), "battle")]
public abstract class CallbackDataBase
{
}
