using JsonKnownTypes;
using Newtonsoft.Json;

namespace TextGameRPG.Scripts.GameCore.Quests.NextStageTriggers;

[JsonConverter(typeof(JsonKnownTypesConverter<TriggerBase>))]
public abstract class TriggerBase
{
    public abstract TriggerType triggerType { get; }
    public int nextStage = -1;

    public abstract bool TryInvoke();
}
