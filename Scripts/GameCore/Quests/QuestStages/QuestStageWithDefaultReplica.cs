using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Dialogs.Quests;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestStages
{
    public enum ReplicaType : byte
    {
        StartTravelToEnemy = 0,
        BattleWin = 1,
        BattleLose = 2,
    }

    public static class ReplicaTypeExtensions
    {
        public static string GetKey(this ReplicaType replicaType)
        {
            switch (replicaType)
            {
                case ReplicaType.StartTravelToEnemy:
                    return "start_travel_to_enemy";
                case ReplicaType.BattleWin:
                    return "battle_win";
                case ReplicaType.BattleLose:
                    return "battle_lose";
            }
            return "UNKNOWN_REPLICA_TYPE";
        }
    }


    [JsonObject]
    public class QuestStageWithDefaultReplica : QuestStage
    {
        [JsonProperty]
        public ReplicaType replicaType { get; set; } = ReplicaType.StartTravelToEnemy;
        [JsonProperty]
        public int nextStage { get; set; }

        public override async Task InvokeStage(GameSession session)
        {
            var replica = new Replica
            {
                characterType = Characters.CharacterType.None,
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

            await new QuestReplicaDialog(session, replica).Start();
        }

        private string GetLocalizationKey()
        {
            var location = quest.questType.GetLocation();
            if (location != null)
            {
                var locNumber = (byte)location;
                string locStr = locNumber < 10 ? "0" + locNumber : locNumber.ToString();
                return string.Format("quest_loc_{0}_{1}", locStr, replicaType.GetKey());
            }

            return string.Format("quest_{0}_{1}", quest.questType, replicaType.GetKey());
        }

    }
}
