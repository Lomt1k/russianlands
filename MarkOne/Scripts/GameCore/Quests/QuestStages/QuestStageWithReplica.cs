using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Quests.Characters;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Quests;

namespace MarkOne.Scripts.GameCore.Quests.QuestStages;

[JsonObject]
public class QuestStageWithReplica : QuestStage
{
    public Replica replica { get; set; } = new Replica();

    public override async Task InvokeStage(GameSession session)
    {
        await new QuestReplicaDialog(session, replica).Start().FastAwait();
    }
}

[JsonObject]
public class Replica
{
    public CharacterType characterType { get; set; } = CharacterType.None;
    public string localizationKey { get; set; } = string.Empty;
    public string imageKey { get; set; } = string.Empty;
    public List<Answer> answers { get; set; } = new List<Answer>();
}

[JsonObject]
public class Answer
{
    public string comment { get; set; } = "New Answer";
    public string localizationKey { get; set; } = string.Empty;
    public int nextStage { get; set; }

    public bool IsReplyMessageEquals(GameSession session, string reply)
    {
        var localizedReply = Localizations.Localization.Get(session, localizationKey);
        return localizedReply.Equals(reply);
    }
}
