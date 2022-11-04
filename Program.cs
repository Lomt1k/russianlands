using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading.Tasks;

namespace TextGameRPG
{
    public enum AppMode { None, Editor, Bot };

    public class Program
    {
        public static bool isUnixPlatform => Environment.OSVersion.Platform == PlatformID.Unix;
        public static AppMode appMode { get; private set; } = AppMode.None;
        public static bool isConsoleMode { get; private set; }
        public static Window mainWindow { get; set; }
        public readonly static ILog logger = LogManager.GetLogger(typeof(Program));

        public static Action<AppMode>? onSetupAppMode;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureLogger();
            PerformanceMonitor.Start();
            SelectGUIModeAndRun(args);
        }

        private static void SelectGUIModeAndRun(string[] args)
        {
            // Catching exception when application started from .exe (not from console)
            try
            {
                bool withGUI = ConsoleMode.ConsoleHelper.AskYesNo("Start with GUI?");
                if (!withGUI)
                {
                    StartInConsoleMode(args);
                    return;
                }
            }
            catch (Exception ex) { }
            StartAvalonia(args);
        }

        private static void ConfigureLogger()
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
        }

        private static void StartAvalonia(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
            

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();

        public static async void StartInConsoleMode(string[] args)
        {
            isConsoleMode = true;
            SetupAppMode(AppMode.Bot);
            if (!isUnixPlatform) // там менять кодировку не надо
            {
                Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            var gameDataLoader = new ViewModels.ConsoleGameDataLoaderViewModel();
            while (!gameDataLoader.isCompleted)
            {
                await Task.Delay(15);
            }
            new ConsoleMode.ConsoleHandler().Start(args);
        }

        public static void SetupAppMode(AppMode _appMode)
        {
            if (appMode != AppMode.None)
                throw new InvalidOperationException("AppMode can only be installed once");

            appMode = _appMode;
            onSetupAppMode?.Invoke(_appMode);
        }

        public static void SetTitle(string title)
        {
            if (isConsoleMode)
                Console.Title = title;
            else
                mainWindow.Title = title;
        }
    }
}
