using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.GameCore.Locations;
using TextGameRPG.Scripts.TelegramBot.Sessions;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    public class TestCommand : CommandBase
    {
        public override bool isAdminCommand => true;

        public override async Task Execute(GameSession session, string[] args)
        {
            if (args.Length != 1)
                return;

            if (!args[0].TryParse(out Rarity rarity))
                return;

            var settings = LocationsHolder.Get(LocationType.DarkForest).data.itemGenerationSettings;
            var item = ItemGenerationManager.GenerateItem(settings, rarity);

            var text = ItemViewBuilder.Build(session, item);
            await messageSender.SendTextMessage(session.chatId, text);
        }
    }
}
