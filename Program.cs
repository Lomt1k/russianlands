using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using log4net;
using log4net.Config;
using System.IO;

namespace TextGameRPG
{
    internal class Program
    {
        public static Window mainWindow { get; set; }
        public readonly static ILog logger = LogManager.GetLogger(typeof(Program));

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            ConfigureLogger();
            StartAvalonia(args);
        }

        private static void ConfigureLogger()
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            for (int i = 0; i < 10; i++)
            {
                logger.Info(Environment.NewLine);
            }
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
    }
}
