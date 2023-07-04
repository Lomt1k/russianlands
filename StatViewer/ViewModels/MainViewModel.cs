﻿using Avalonia.Controls;
using ReactiveUI;
using StatViewer.Scripts;
using StatViewer.Views;
using System;
using System.Threading.Tasks;

namespace StatViewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private static readonly LoadingView loadingView = new LoadingView();
    private static readonly InterfaceView interfaceView = new InterfaceView();

    private UserControl? _currentView;

    public UserControl? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainViewModel()
    {
        ShowLoadingView("Loading");
        Task.Run(() => Load(Initialize));
    }

    public async Task Load(Func<Task> loadingTask, string text = "Loading")
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

    private async Task Initialize()
    {
        await StatDataBase.ConnectAsync();
        await StatDataBase.RefreshCache();
    }

}
