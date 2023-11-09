using MarkOne.Scripts.GameCore.Sessions;
using System.Text;
using System.Threading.Tasks;

namespace MarkOne.Scripts.GameCore.Commands.Cheats;
public class EmojiCommand : CommandBase
{
    public override CommandGroup commandGroup => CommandGroup.Cheat;

    public override async Task Execute(GameSession session, string[] args)
    {
        if (args.Length == 0)
        {
            await SendManualMessage(session).FastAwait();
            return;
        }

        var input = new StringBuilder();
        var output = new StringBuilder();
        foreach (var str in args)
        {
            input.Append(str);
            foreach (var ch in str)
            {
                output.Append(CharToUnicodeFormat(ch));
            }
        }

        var text = $"<b>Input:</b>\n{input}\n<b>Unicode:</b>\n{output}";
        await messageSender.SendTextMessage(session.chatId, text).FastAwait();
    }

    private static string CharToUnicodeFormat(char c)
    {
        return string.Format(@"\u{0:x4}", (int)c);
    }

    public static async Task SendManualMessage(GameSession session)
    {
        var text = new StringBuilder()
            .AppendLine("Usage:".Bold())
            .AppendLine()
            .AppendLine($"/emoji [emoji]")
            .ToString();

        await messageSender.SendTextMessage(session.chatId, text).FastAwait();
    }

}
