using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GameDataEditor.ViewModels;
using GameDataEditor.Views;

namespace GameDataEditor;

public partial class App : Application
{
    public static MainWindow mainWindow;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            mainWindow = new MainWindow
            {
                DataContext = new MainView() { DataContext = new MainViewModel() }
            };
            desktop.MainWindow = mainWindow;
        }
        else
        {
            throw new System.Exception("GameDataEditor can be started only at Desktop");
        }

        base.OnFrameworkInitializationCompleted();
    }
}
