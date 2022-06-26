using Newtonsoft.Json;
using JsonKnownTypes;
using System.Threading.Tasks;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.GameCore.Quests.StageActions
{
    [JsonConverter(typeof(JsonKnownTypesConverter<StageActionBase>))]
    public abstract class StageActionBase
    {
        public abstract Task Execute(GameSession session);
    }
}
