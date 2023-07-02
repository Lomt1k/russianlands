using Avalonia.Controls;
using ReactiveUI;
using StatViewer.Scripts;
using StatViewer.Views;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private UserControl? _currentView;

    public UserControl? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public string Greeting => "Welcome to Avalonia!";

    public MainViewModel()
    {
        _currentView = new LoadingView();
        Task.Run(Start);
    }

    private async Task Start()
    {
        await StatDataBase.ConnectAsync();
        await StatDataBase.RefreshCache();
    }

}
