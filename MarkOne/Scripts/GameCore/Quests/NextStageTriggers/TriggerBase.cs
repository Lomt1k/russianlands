using JsonKnownTypes;
using Newtonsoft.Json;

namespace MarkOne.Scripts.GameCore.Quests.NextStageTriggers;

[JsonConverter(typeof(JsonKnownTypesConverter<TriggerBase>))]
public abstract class TriggerBase
{
    public abstract TriggerType triggerType { get; }
    public int nextStage { get; set; } = -1;

    public abstract bool TryInvoke();
}
