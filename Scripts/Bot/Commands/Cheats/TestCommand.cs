using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators;
using TextGameRPG.Scripts.Bot.Sessions;

namespace TextGameRPG.Scripts.Bot.Commands.Cheats
{
    public class TestCommand : CommandBase
    {
        public override CommandGroup commandGroup => CommandGroup.Cheat;

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
