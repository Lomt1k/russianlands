using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextGameRPG.Scripts.Bot;
using TextGameRPG.Scripts.Bot.Sessions;
using TextGameRPG.Scripts.GameCore.Services;

namespace TextGameRPG.ConsoleMode
{
    // Для запуска приложения в консольном режиме
    public class ConsoleHandler
    {
        public const string botDataFolder = "botData";

        private static readonly SessionManager sessionManager = Services.Get<SessionManager>();
        private static readonly PerformanceManager pm = Services.Get<PerformanceManager>();

        private static Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>
        {
            {"start", (args) => StartBotCommand(args) },
            {"stop", (args) => StopBotCommand(args) },
            {"exit", (args) => Shutdown(args) },
            {"status", (args) => StatusCommand(args) },
            {"collect", (args) => CollectCommand(args) }
        };

        public void Start(string[] args)
        {
            Console.WriteLine("Started with console...");

            if (args.Length > 0)
            {
                var botDataInput = args[0].TrimStart('-');
                var botDataPath = Path.Combine(botDataFolder, botDataInput);
                StartWithBotData(botDataPath);
            }
            else
            {
                StartWithBotDataSelection();
            }

            Console.WriteLine("Shutdown...");
        }

        private void StartWithBotDataSelection()
        {
            if (!Directory.Exists(botDataFolder))
            {
                Directory.CreateDirectory(botDataFolder);
            }

            Console.WriteLine("Please select bot data...");
            string botDataPath = string.Empty;
            while (string.IsNullOrEmpty(botDataPath) || !Directory.Exists(botDataPath))
            {
                Console.WriteLine("All bot data:");
                var allBotData = Directory.GetDirectories(botDataFolder);
                foreach (var botData in allBotData)
                {
                    var shortName = botData.Replace(botDataFolder, string.Empty).TrimStart('\\', '/');
                    Console.WriteLine($"* {shortName}");
                }

                var input = Console.ReadLine();
                if (input.ToLower().Equals("exit"))
                    return; //exit

                botDataPath = Path.Combine(botDataFolder, input);
            }

            StartWithBotData(botDataPath);
        }

        private void StartWithBotData(string botDataPath)
        {
            BotController.Init(botDataPath);
            BotController.StartListening();
            ListenConsoleCommands();
        }

        private void ListenConsoleCommands()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                var args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var command = args[0].ToLower();
                args = args.Skip(1).ToArray();
                HandleCommand(command, args);
            }
        }

        private void HandleCommand(string command, string[] args)
        {
            if (commands.TryGetValue(command, out var commandFunc))
            {
                commandFunc(args);
                return;
            }
            Console.WriteLine($"Unknown command '{command}'");
        }

        private static void StartBotCommand(string[] args)
        {
            BotController.StartListening();
        }

        private static void StopBotCommand(string[] args)
        {
            BotController.StopListening();
        }

        private static void Shutdown(string[] args)
        {
            BotController.Shutdown();
        }

        private static void CollectCommand(string[] args)
        {
            GC.Collect();
            Console.WriteLine("GC.Collect invoked");
        }

        private static void StatusCommand(string[] args)
        {
            if (pm == null)
                return;

            Console.WriteLine($"Status: {pm.currentState}");
            Console.WriteLine(pm.debugInfo);

            Console.WriteLine();
            var allSessions = sessionManager.GetAllSessions();
            Console.WriteLine($"Active sessions: {allSessions.Count}");
            var dtNow = DateTime.UtcNow;
            var recentlyActive = allSessions.Where(x => (dtNow - x.lastActivityTime).TotalMinutes < 5).Count();
            Console.WriteLine($"Now playing: {recentlyActive}");
        }





    }
}
