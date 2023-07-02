using Avalonia.Controls;
using ReactiveUI;

namespace GameDataEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    private UserControl _mainContentView;

    public UserControl mainContentView
    {
        get => _mainContentView;
        set => this.RaiseAndSetIfChanged(ref _mainContentView, value);
    }

    public MainViewModel()
    {
        LaunchEditor();
    }

    private void LaunchEditor()
    {
        mainContentView = new Views.Editor.MainEditorView();
        mainContentView.DataContext = new Editor.MainEditorViewModel();
    }
}
