using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Quests.Characters;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Quests;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    public enum ReplicaType : byte
    {
        StartTravelToEnemyLoc01 = 0,
        BattleWin = 1,
        BattleLose = 2,
    }

    [JsonObject]
    public class QuestStageWithDefaultReplica : QuestStage
    {
        [JsonProperty]
        public ReplicaType replicaType { get; set; } = ReplicaType.StartTravelToEnemyLoc01;
        [JsonProperty]
        public int nextStage { get; set; }

        [JsonIgnore]
        public Replica replica = new Replica();

        public override async Task InvokeStage(GameSession session)
        {
            await new QuestReplicaDialog(session, replica).Start();
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
                case ReplicaType.BattleWin:
                    return "quest_default_replica_battle_win";
                case ReplicaType.BattleLose:
                    return "quest_default_replica_battle_lose";
                case ReplicaType.StartTravelToEnemyLoc01:
                    return $"quest_default_replica_start_travel_to_enemy_loc_01";
            }

            var errorMessage = $"Unknown replicaType {replicaType} for Default Replica";
            Program.logger.Error(errorMessage);
            return errorMessage;
        }

    }
}
