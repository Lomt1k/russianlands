using Avalonia.Controls;
using ReactiveUI;
using MarkOne.Views;

namespace MarkOne.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private UserControl _mainContentView;

    public UserControl mainContentView
    {
        get => _mainContentView;
        set => this.RaiseAndSetIfChanged(ref _mainContentView, value);
    }

    public MainWindowViewModel()
    {
        LaunchEditor();
    }

    private void LaunchEditor()
    {
        mainContentView = new Views.Editor.MainEditorView();
        mainContentView.DataContext = new Editor.MainEditorViewModel();
        //MainWindow.instance.WindowState = WindowState.Maximized;
    }



}
