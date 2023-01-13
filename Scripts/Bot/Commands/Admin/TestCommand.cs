using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands.Admin
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

            var townHallLevel = 2;
            var item = ItemGenerationManager.GenerateItem(townHallLevel, rarity);

            var text = ItemViewBuilder.Build(session, item);
            await messageSender.SendTextMessage(session.chatId, text);
        }
    }
}
