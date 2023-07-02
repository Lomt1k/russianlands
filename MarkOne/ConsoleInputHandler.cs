using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkOne.Scripts.Bot;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Sessions;

namespace MarkOne;

public class ConsoleInputHandler
{
    public static readonly string botDataFolder = Path.Combine("Assets", "botData");

    private static readonly SessionManager sessionManager = ServiceLocator.Get<SessionManager>();
    private static readonly PerformanceManager pm = ServiceLocator.Get<PerformanceManager>();

    private static readonly Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>
    {
        {"start", StartBotCommand },
        {"stop", StopBotCommand },
        {"exit", Shutdown },
        {"status", StatusCommand },
        {"collect", CollectCommand }
    };

    public void Start(string[] args)
    {
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
        var botDataPath = string.Empty;
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
