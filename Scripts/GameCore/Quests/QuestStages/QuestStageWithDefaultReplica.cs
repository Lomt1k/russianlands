using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.Bot.Dialogs.Quests;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
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
        public Replica replica = new Replica();

        public override async Task InvokeStage(GameSession session)
        {
            await new QuestReplicaDialog(session, replica).Start().ConfigureAwait(false);
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

    }
}
