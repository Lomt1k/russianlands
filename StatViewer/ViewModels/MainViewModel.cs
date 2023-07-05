using Avalonia.Controls;
using ReactiveUI;
using StatViewer.Scripts;
using StatViewer.Views;
using System;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private static readonly LoadingView loadingView = new();
    private static readonly InterfaceView interfaceView = new();

    private UserControl? _currentView;

    public UserControl? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainViewModel()
    {
        ShowLoadingView("Loading");
        Task.Run(() => StartLoading(InitializeDatabase));
    }

    public async Task StartLoading(Func<Task> loadingTask, string text = "Loading")
    {
        ShowLoadingView(text);
        await loadingTask();
        ShowInterfaceView();
    }

    private void ShowLoadingView(string text)
    {
        loadingView.vm.SetText(text);
        CurrentView = loadingView;
    }

    private void ShowInterfaceView()
    {
        CurrentView = interfaceView;
    }

    private async Task InitializeDatabase()
    {
        await StatDataBase.ConnectAsync();
        await StatDataBase.RefreshCache();
    }

}
