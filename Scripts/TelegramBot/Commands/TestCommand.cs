using System.Threading.Tasks;
using TextGameRPG.Scripts.GameCore.Items;
using TextGameRPG.Scripts.GameCore.Items.Generators.CodeGenerators;
using TextGameRPG.Scripts.TelegramBot.Sessions;
using TextGameRPG.Scripts.Utils;

namespace TextGameRPG.Scripts.TelegramBot.Commands
{
    internal class TestCommand : CommandBase
    {
        public override async Task Execute(GameSession session, string[] args)
        {
            if (args.Length != 1)
                return;

            if (!args[0].TryParse(out ItemRarity rarity))
                return;

            var code = new ScrollCodeGenerator(ItemType.Scroll, rarity, 1, 20).GetCode();
            await messageSender.SendTextMessage(session.chatId, code);

            var item = new InventoryItem(code);
            var text = ItemViewBuilder.Build(session, item);
            await messageSender.SendTextMessage(session.chatId, text);
        }
    }
}
