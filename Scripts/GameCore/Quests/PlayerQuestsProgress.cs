using Newtonsoft.Json;
using System.Collections.Generic;

namespace TextGameRPG.Scripts.GameCore.Quests;

[JsonObject]
public class PlayerQuestsProgress
{
    [JsonProperty("focus")]
    private QuestId? focusedQuest = null;

    [JsonProperty]
    private readonly Dictionary<ushort, int> stages = new Dictionary<ushort, int>();

    public bool IsStarted(QuestId questId)
    {
        return stages.ContainsKey((ushort)questId);
    }

    public int GetStage(QuestId questId)
    {
        return IsStarted(questId) ? stages[(ushort)questId] : 0;
    }

    public void SetStage(QuestId questId, int stage)
    {
        stages[(ushort)questId] = stage;
        if (stage > 0)
        {
            focusedQuest = questId;
        }
        else if (focusedQuest == questId)
        {
            focusedQuest = null;
        }
    }

    public bool IsCompleted(QuestId questId)
    {
        return GetStage(questId) == -1;
    }

    public QuestId? GetFocusedQuest()
    {
        return focusedQuest;
    }

    public void Cheat_SetCurrentQuest(QuestId questId, int stageId)
    {
        stages.Clear();
        var currentQuest = (ushort)questId;

        //setup completed quests
        for (var i = (ushort)QuestId.MainQuest; i < currentQuest; i++)
        {
            stages.Add(i, -1);
        }

        stages[currentQuest] = stageId;
        focusedQuest = questId;
    }

}
