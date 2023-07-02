using log4net;
using log4net.Config;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using System;
using System.IO;

namespace MarkOne;

// TODO: избавиться от AppMode. Пережиток прошлого...
public enum AppMode { None, Editor, PlayMode };

public class Program
{
    public const string cacheDirectory = "cache";

    public static bool isUnixPlatform => Environment.OSVersion.Platform == PlatformID.Unix;
    public static AppMode appMode { get; private set; } = AppMode.None;
    public readonly static ILog logger = LogManager.GetLogger(typeof(Program));

    public static Action<AppMode>? onSetupAppMode;

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
        StartInConsoleMode(args);
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

    private static void StartInConsoleMode(string[] args)
    {
        appMode = AppMode.PlayMode;
        onSetupAppMode?.Invoke(appMode);
        new ConsoleInputHandler().Start(args);
    }

    public static void SetTitle(string title)
    {
        Console.Title = title;
    }
}
