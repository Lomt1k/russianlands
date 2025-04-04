﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MarkOne.Scripts.GameCore.Quests.Characters;
using MarkOne.Scripts.GameCore.Sessions;
using MarkOne.Scripts.GameCore.Dialogs.Quests;

namespace MarkOne.Scripts.GameCore.Quests.QuestStages;

public enum ReplicaType : byte
{
    BattleLose = 0,
    StartTravelToEnemyLoc01 = 1,
    StartTravelToEnemyLoc02 = 2,
    StartTravelToEnemyLoc03 = 3,
    StartTravelToEnemyLoc04 = 4,
    StartTravelToEnemyLoc05 = 5,
    StartTravelToEnemyLoc06 = 6,
    StartTravelToEnemyLoc07 = 7,
}

[JsonObject]
public class QuestStageWithDefaultReplica : QuestStage
{
    [JsonProperty]
    public ReplicaType replicaType { get; set; }
    [JsonProperty]
    public int nextStage { get; set; }

    [JsonIgnore]
    public Replica replica = new();

    public override async Task InvokeStage(GameSession session)
    {
        await new QuestReplicaDialog(session, replica).Start().FastAwait();
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        BuildReplica();
    }

    private void BuildReplica()
    {
        replica = new Replica
        {
            characterType = GetCharacterType(),
            localizationKey = GetLocalizationKey(),
            imageKey = GetImageKey(),
            answers = new List<Answer>
            {
                new Answer
                {
                    nextStage = nextStage,
                    localizationKey = "menu_item_continue_button"
                }
            }
        };
    }
    private CharacterType GetCharacterType()
    {
        switch (replicaType)
        {
            case ReplicaType.BattleLose: return CharacterType.Vasilisa;
            default: return CharacterType.None;
        }
    }

    private string GetLocalizationKey()
    {
        switch (replicaType)
        {
            case ReplicaType.BattleLose:
                return "quest_default_replica_battle_lose";
            case ReplicaType.StartTravelToEnemyLoc01:
                return $"quest_default_replica_start_travel_to_enemy_loc_01";
            case ReplicaType.StartTravelToEnemyLoc02:
                return $"quest_default_replica_start_travel_to_enemy_loc_02";
            case ReplicaType.StartTravelToEnemyLoc03:
                return $"quest_default_replica_start_travel_to_enemy_loc_03";
            case ReplicaType.StartTravelToEnemyLoc04:
                return $"quest_default_replica_start_travel_to_enemy_loc_04";
            case ReplicaType.StartTravelToEnemyLoc05:
                return $"quest_default_replica_start_travel_to_enemy_loc_05";
            case ReplicaType.StartTravelToEnemyLoc06:
                return $"quest_default_replica_start_travel_to_enemy_loc_06";
            case ReplicaType.StartTravelToEnemyLoc07:
                return $"quest_default_replica_start_travel_to_enemy_loc_07";

            default:
                return "Default Replica";
        }
    }

    private string GetImageKey()
    {
        switch (replicaType)
        {
            case ReplicaType.StartTravelToEnemyLoc01:
                return $"loc_01.webp";
            case ReplicaType.StartTravelToEnemyLoc02:
                return $"loc_02.webp";
            case ReplicaType.StartTravelToEnemyLoc03:
                return $"loc_03.webp";
            case ReplicaType.StartTravelToEnemyLoc04:
                return $"loc_04.webp";
            case ReplicaType.StartTravelToEnemyLoc05:
                return $"loc_05.webp";
            case ReplicaType.StartTravelToEnemyLoc06:
                return $"loc_06.webp";
            case ReplicaType.StartTravelToEnemyLoc07:
                return $"loc_07.webp";

            default:
                return string.Empty;
        }
    }

}
