using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GameDataEditor.ViewModels;
using GameDataEditor.Views;
using MarkOne.Scripts.GameCore.Services;
using MarkOne.Scripts.GameCore.Services.GameData;
using System;

namespace GameDataEditor;

public partial class App : Application
{
    public static MainWindow mainWindow { get; private set; }

    public override void Initialize()
    {
        var gameDataHolder = ServiceLocator.Get<GameDataHolder>();
        gameDataHolder.LoadAllData();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            desktop.MainWindow = mainWindow;
        }
        else
        {
            throw new System.Exception("Only desktop supported");
        }

        base.OnFrameworkInitializationCompleted();
    }
}
