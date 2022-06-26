using Newtonsoft.Json;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.QuestActions
{
    [JsonObject]
    internal class RestoreFullHealthAction : StageActionBase
    {
        public override async Task Execute(GameSession session)
        {
            session.player.unitStats.SetFullHealth();
        }
    }
}
