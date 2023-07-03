using log4net;
using log4net.Config;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using System;
using System.IO;

namespace MarkOne;

public class Program
{
    public const string cacheDirectory = "cache";

    public static bool isUnixPlatform => Environment.OSVersion.Platform == PlatformID.Unix;
    public readonly static ILog logger = LogManager.GetLogger(typeof(Program));

    public static Action? onBotAppStarted;
    public static bool isBotAppStarted;

    public static void Main(string[] args)
    {
        if (!isUnixPlatform) // на юниксе менять кодировку не надо
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
        }

        PrepareCacheFolder();
        ConfigureLogger();
        PerformanceMonitor.Start();
        LoadGameData();
        isBotAppStarted = true;
        onBotAppStarted?.Invoke();
        new ConsoleInputHandler().Start(args);
    }

    private static void PrepareCacheFolder()
    {
        if (Directory.Exists(cacheDirectory))
        {
            Directory.Delete(cacheDirectory, true);
        }
        Directory.CreateDirectory(cacheDirectory);
    }

    private static void ConfigureLogger()
    {
        var configPath = Path.Combine("Assets", "log4net.config");
        XmlConfigurator.Configure(new FileInfo(configPath));
    }

    private static void LoadGameData()
    {
        var gameDataHolder = ServiceLocator.Get<GameDataHolder>();
        gameDataHolder.LoadAllData();
    }

    public static void SetTitle(string title)
    {
        Console.Title = title;
    }
}
